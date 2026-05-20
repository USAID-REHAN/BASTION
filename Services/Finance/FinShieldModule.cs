using BASTION.Data;
using BASTION.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BASTION.Services.Finance;

/// <summary>
/// IModule implementation for FinShield — financial fraud protection module.
/// </summary>
public class FinShieldModule : IModule
{
    private readonly BastionDbContext _db;

    public FinShieldModule(BastionDbContext db) => _db = db;

    public string Name => "FinShield";
    public string Icon => "bi-currency-exchange";
    public string RouteUrl => "/finshield";
    public string Description => "Fraud protection — URL scanner, message analyzer, scam feed, education";
    public string PrimaryHex => "#059669";
    public string CssClass => "module-fin";
    public int Order => 5;

    public async Task<int> GetScoreAsync(string userId)
    {
        var since = DateTime.UtcNow.AddDays(-30);
        var urlScans = await _db.UrlScanLogs
            .Where(s => s.Email == userId && s.ScannedAt >= since)
            .ToListAsync();
        var paymentScans = await _db.PaymentPageAnalyses
            .Where(s => s.Email == userId && s.AnalyzedAt >= since)
            .ToListAsync();

        var total = urlScans.Count + paymentScans.Count;
        if (total == 0) return 0;

        var highRisk = urlScans.Count(s => s.RiskScore >= 70) + paymentScans.Count(s => s.RiskScore >= 70);
        return Math.Clamp(35 + Math.Min(total * 10, 50) - (highRisk * 5), 0, 100);
    }
}
