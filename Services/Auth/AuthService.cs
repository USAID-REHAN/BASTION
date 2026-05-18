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
    private static readonly List<SessionInfo> _sessions = new();
    private UserAccount? _currentUser;
    public event Action? OnAuthStateChanged;
    public static event Action<string>? OnSessionRevoked;

    public bool IsAuthenticated => _currentUser != null;
    public bool IsAdmin => _currentUser?.Role == "Admin";
    public UserAccount? CurrentUser => _currentUser;

    public AuthService(BastionDbContext dbContext)
    {
        _dbContext = dbContext;
        OnSessionRevoked += HandleSessionRevoked;
    }

    private void HandleSessionRevoked(string email)
    {
        if (_currentUser?.Email == email)
        {
            _currentUser = null;
            OnAuthStateChanged?.Invoke();
        }
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

        _sessions.Add(new SessionInfo
        {
            UserEmail = email,
            Device = "Web Browser",
            Location = user.City ?? "Unknown",
            LoginTime = DateTime.UtcNow,
            IsActive = true
        });

        OnAuthStateChanged?.Invoke();
        return (true, "Welcome back, " + user.FullName + "!");
    }

    public void Logout()
    {
        if (_currentUser != null)
        {
            var session = _sessions.LastOrDefault(s => s.UserEmail == _currentUser.Email && s.IsActive);
            if (session != null) session.IsActive = false;
        }
        _currentUser = null;
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
        return _sessions.Where(s => s.UserEmail == _currentUser.Email).OrderByDescending(s => s.LoginTime).ToList();
    }

    public List<SessionInfo> GetAllSessions()
    {
        return _sessions.OrderByDescending(s => s.LoginTime).ToList();
    }

    public void RevokeSession(SessionInfo session)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == session.UserEmail);
        if (user != null && user.Role == "Admin") return; // Cannot revoke admin sessions

        session.IsActive = false;
        OnSessionRevoked?.Invoke(session.UserEmail);
    }

    public void Dispose()
    {
        OnSessionRevoked -= HandleSessionRevoked;
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
}
