using System.Security.Claims;

namespace BASTION.Services.Auth;

/// <summary>
/// In-memory authentication service for BASTION.
/// Manages user registration, login, and session state.
/// Will be replaced with ASP.NET Identity + JWT when database layer is integrated.
/// </summary>
public class AuthService
{
    private static readonly Dictionary<string, UserAccount> _users = new()
    {
        ["admin@bastion.app"] = new UserAccount
        {
            Email = "admin@bastion.app",
            FullName = "BASTION Admin",
            PasswordHash = HashPassword("Admin@123"),
            Role = "Admin",
            City = "Islamabad",
            CreatedAt = DateTime.UtcNow,
            MfaEnabled = false
        }
    };

    private static readonly List<SessionInfo> _sessions = new();
    private UserAccount? _currentUser;
    public event Action? OnAuthStateChanged;

    public bool IsAuthenticated => _currentUser != null;
    public bool IsAdmin => _currentUser?.Role == "Admin";
    public UserAccount? CurrentUser => _currentUser;

    public (bool Success, string Message) Register(string fullName, string email, string password, string city)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email and password are required.");

        if (_users.ContainsKey(email.ToLower()))
            return (false, "An account with this email already exists.");

        if (password.Length < 8)
            return (false, "Password must be at least 8 characters.");

        var user = new UserAccount
        {
            Email = email.ToLower(),
            FullName = fullName,
            PasswordHash = HashPassword(password),
            Role = "User",
            City = city,
            CreatedAt = DateTime.UtcNow,
            MfaEnabled = false
        };

        _users[email.ToLower()] = user;
        return (true, "Account created successfully!");
    }

    public (bool Success, string Message) Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email and password are required.");

        email = email.ToLower();
        if (!_users.TryGetValue(email, out var user))
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
                return (false, "Account locked for 15 minutes after 5 failed attempts.");
            }
            return (false, $"Invalid email or password. {5 - user.FailedAttempts} attempt(s) remaining.");
        }

        // Successful login
        user.FailedAttempts = 0;
        user.IsLockedOut = false;
        user.LockoutEnd = null;
        _currentUser = user;

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
        return true;
    }

    public List<SessionInfo> GetActiveSessions()
    {
        if (_currentUser == null) return new();
        return _sessions.Where(s => s.UserEmail == _currentUser.Email).OrderByDescending(s => s.LoginTime).ToList();
    }

    public void RevokeSession(SessionInfo session)
    {
        session.IsActive = false;
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
        // Simple hash for development — will be replaced with BCrypt/Argon2
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password + "BASTION_SALT_2026");
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}

public class UserAccount
{
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User";
    public string? City { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool MfaEnabled { get; set; }
    public string? MfaSecret { get; set; }
    public int FailedAttempts { get; set; }
    public bool IsLockedOut { get; set; }
    public DateTime? LockoutEnd { get; set; }
}

public class SessionInfo
{
    public string UserEmail { get; set; } = "";
    public string Device { get; set; } = "";
    public string Location { get; set; } = "";
    public DateTime LoginTime { get; set; }
    public bool IsActive { get; set; }
}
