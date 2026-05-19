using System;
using System.ComponentModel.DataAnnotations;

namespace BASTION.Models;

public class UserBadge
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty; // FK to UserAccount (using Email as key)
    [Required]
    public string BadgeName { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
}

public class Lab
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int XpReward { get; set; }
}

public class UserLabCompletion
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty; // FK to UserAccount
    public int LabId { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}
