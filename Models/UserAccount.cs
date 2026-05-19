namespace BASTION.Models;

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
