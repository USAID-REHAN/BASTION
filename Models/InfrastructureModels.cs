using System;
using System.ComponentModel.DataAnnotations;

namespace BASTION.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string? Email { get; set; } // Nullable for anonymous events
    [Required]
    public string Action { get; set; } = string.Empty; // Login, File Upload, Scan
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string Details { get; set; } = string.Empty;
}

public class UserPreference
{
    [Key]
    public string Email { get; set; } = string.Empty; // PK and FK to UserAccount
    public string Theme { get; set; } = "Light"; // Light, Dark, System
    public string AccentColor { get; set; } = "#007bff"; // Default blue or custom from color wheel
    public bool NotificationsEnabled { get; set; } = true;
}

public class Notification
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty; // FK to UserAccount
    [Required]
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class UserSession
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty; // FK to UserAccount
    public string Device { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;
}
