using System;
using System.ComponentModel.DataAnnotations;

namespace BASTION.Models;

public class UrlScanLog
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty; // FK to UserAccount
    [Required]
    public string Url { get; set; } = string.Empty;
    [Required]
    public string Verdict { get; set; } = string.Empty; // Safe, Caution, Danger
    public int RiskScore { get; set; }
    public string FlagsJson { get; set; } = "[]"; // Detailed flags
    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
}

public class PaymentPageAnalysis
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty; // FK to UserAccount
    [Required]
    public string PageUrl { get; set; } = string.Empty;
    public string VisionReportSummary { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string AnomaliesJson { get; set; } = "[]";
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}
