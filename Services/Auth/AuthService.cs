using System.Security.Claims;
using BASTION.Data;
using BASTION.Models;
using Microsoft.EntityFrameworkCore;

namespace BASTION.Services.Auth;

/// <summary>
/// Database-backed authentication service for BASTION.
/// Manages user registration, login, and session state.
/// </summary>
public class AuthService : IDisposable
{
    private readonly BastionDbContext _dbContext;
    private UserAccount? _currentUser;
    private int? _currentSessionId;
    public event Action? OnAuthStateChanged;
    public static event Action<string, bool>? OnSessionRevoked;
    public string LastSignOutReason { get; private set; } = "revoked";

    public bool IsAuthenticated => _currentUser != null;
    public bool IsAdmin => _currentUser?.Role == "Admin";
    public UserAccount? CurrentUser => _currentUser;

    public AuthService(BastionDbContext dbContext)
    {
        _dbContext = dbContext;
        OnSessionRevoked += HandleSessionRevoked;
    }

    private void HandleSessionRevoked(string email, bool isBan)
    {
        if (_currentUser?.Email == email)
        {
            _currentUser = null;
            LastSignOutReason = isBan ? "banned" : "revoked";
            OnAuthStateChanged?.Invoke();
        }
    }

    public static void TriggerSessionRevocation(string email, bool isBan = false)
    {
        OnSessionRevoked?.Invoke(email, isBan);
    }

    public (bool Success, string Message) Register(string fullName, string email, string password, string city)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email and password are required.");

        email = email.ToLower();
        if (_dbContext.Users.Any(u => u.Email == email))
            return (false, "An account with this email already exists.");

        if (password.Length < 8)
            return (false, "Password must be at least 8 characters.");

        var user = new UserAccount
        {
            Email = email,
            FullName = fullName,
            PasswordHash = HashPassword(password),
            Role = "User",
            City = city,
            CreatedAt = DateTime.UtcNow,
            MfaEnabled = false
        };

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return (true, "Account created successfully!");
    }

    public (bool Success, string Message) Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email and password are required.");

        email = email.ToLower();
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
            return (false, "Invalid email or password.");

        if (user.IsLockedOut && user.LockoutEnd > DateTime.UtcNow)
        {
            var remaining = (user.LockoutEnd.Value - DateTime.UtcNow).Minutes + 1;
            return (false, $"Account locked. Try again in {remaining} minute(s).");
        }

        if (user.PasswordHash != HashPassword(password))
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= 5)
            {
                user.IsLockedOut = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                user.FailedAttempts = 0;
            }
            _dbContext.SaveChanges();
            
            if (user.IsLockedOut)
                return (false, "Account locked for 15 minutes after 5 failed attempts.");
                
            return (false, $"Invalid email or password. {5 - user.FailedAttempts} attempt(s) remaining.");
        }

        // Successful login
        user.FailedAttempts = 0;
        user.IsLockedOut = false;
        user.LockoutEnd = null;
        _currentUser = user;
        _dbContext.SaveChanges();

        // Revoke all previous active sessions for this user to keep active sessions clean and duplicate-free
        var previousActiveSessions = _dbContext.UserSessions
            .Where(s => s.Email == email && !s.IsRevoked)
            .ToList();
        foreach (var oldSession in previousActiveSessions)
        {
            oldSession.IsRevoked = true;
        }
        _dbContext.SaveChanges();

        var newSession = new UserSession
        {
            Email = email,
            Device = "Web Browser",
            Location = user.City ?? "Unknown",
            LoginTime = DateTime.UtcNow,
            IsRevoked = false
        };
        _dbContext.UserSessions.Add(newSession);
        _dbContext.SaveChanges();

        _currentSessionId = newSession.Id;

        OnAuthStateChanged?.Invoke();
        return (true, "Welcome back, " + user.FullName + "!");
    }

    public void Logout()
    {
        if (_currentUser != null)
        {
            var session = _dbContext.UserSessions
                .OrderByDescending(s => s.LoginTime)
                .FirstOrDefault(s => s.Email == _currentUser.Email && !s.IsRevoked);
                
            if (session != null)
            {
                session.IsRevoked = true;
                _dbContext.SaveChanges();
            }
        }
        _currentUser = null;
        _currentSessionId = null;
        OnAuthStateChanged?.Invoke();
    }

    public bool ChangePassword(string currentPassword, string newPassword)
    {
        if (_currentUser == null) return false;
        if (_currentUser.PasswordHash != HashPassword(currentPassword)) return false;
        
        _currentUser.PasswordHash = HashPassword(newPassword);
        _dbContext.SaveChanges();
        return true;
    }

    public List<SessionInfo> GetActiveSessions()
    {
        if (_currentUser == null) return new();
        return _dbContext.UserSessions
            .Where(s => s.Email == _currentUser.Email && !s.IsRevoked)
            .OrderByDescending(s => s.LoginTime)
            .Select(s => new SessionInfo
            {
                UserEmail = s.Email,
                Device = s.Device,
                Location = s.Location,
                LoginTime = s.LoginTime,
                IsActive = !s.IsRevoked,
                IsRevokedByAdmin = s.IsRevokedByAdmin
            })
            .ToList();
    }

    public List<SessionInfo> GetAllSessions()
    {
        return _dbContext.UserSessions
            .OrderByDescending(s => s.LoginTime)
            .Select(s => new SessionInfo
            {
                UserEmail = s.Email,
                Device = s.Device,
                Location = s.Location,
                LoginTime = s.LoginTime,
                IsActive = !s.IsRevoked,
                IsRevokedByAdmin = s.IsRevokedByAdmin
            })
            .ToList();
    }

    public void RevokeSession(SessionInfo session)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == session.UserEmail);
        if (user != null && user.Role == "Admin") return; // Cannot revoke admin sessions

        var dbSession = _dbContext.UserSessions
            .FirstOrDefault(s => s.Email == session.UserEmail && s.LoginTime == session.LoginTime);
            
        if (dbSession != null)
        {
            dbSession.IsRevoked = true;
            dbSession.IsRevokedByAdmin = true; // Explicitly marked as admin revoked!
            _dbContext.SaveChanges();
        }

        session.IsActive = false;
        session.IsRevokedByAdmin = true;
        OnSessionRevoked?.Invoke(session.UserEmail, false);
    }

    public void Dispose()
    {
        OnSessionRevoked -= HandleSessionRevoked;

        if (_currentSessionId.HasValue)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<BastionDbContext>();
                optionsBuilder.UseSqlite("Data Source=bastion.db");
                using var db = new BastionDbContext(optionsBuilder.Options);
                var session = db.UserSessions.Find(_currentSessionId.Value);
                if (session != null)
                {
                    session.IsRevoked = true;
                    db.SaveChanges();
                }
            }
            catch
            {
                // Prevent failures in Dispose during server cleanup
            }
        }
    }

    public ClaimsPrincipal GetClaimsPrincipal()
    {
        if (_currentUser == null)
            return new ClaimsPrincipal(new ClaimsIdentity());

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, _currentUser.FullName),
            new(ClaimTypes.Email, _currentUser.Email),
            new(ClaimTypes.Role, _currentUser.Role)
        };
        var identity = new ClaimsIdentity(claims, "BastionAuth");
        return new ClaimsPrincipal(identity);
    }

    private static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password + "BASTION_SALT_2026");
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}

public class SessionInfo
{
    public string UserEmail { get; set; } = "";
    public string Device { get; set; } = "";
    public string Location { get; set; } = "";
    public DateTime LoginTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsRevokedByAdmin { get; set; }
}
