using System;
using System.ComponentModel.DataAnnotations;

namespace BASTION.Models;

public class SecurityScanLog
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty; // FK to UserAccount
    [Required]
    public string ScanType { get; set; } = string.Empty; // IP, Breach, etc.
    [Required]
    public string Target { get; set; } = string.Empty; // The IP or email checked
    [Required]
    public string Result { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
