using BASTION.Data;
using BASTION.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BASTION.Services.Security;

/// <summary>
/// IModule implementation for CyberShield — registers module identity and exposes GetScoreAsync.
/// </summary>
public class CyberShieldModule : IModule
{
    private readonly BastionDbContext _db;

    public CyberShieldModule(BastionDbContext db) => _db = db;

    public string Name => "CyberShield";
    public string Icon => "bi-shield-check";
    public string RouteUrl => "/cybershield";
    public string Description => "Device protection — PowerShell audit, IP checker, password breach, chatbot";
    public string PrimaryHex => "#0891B2";
    public string CssClass => "module-cyber";
    public int Order => 3;

    public async Task<int> GetScoreAsync(string userId)
    {
        var recentScans = await _db.SecurityScanLogs
            .Where(s => s.Email == userId && s.CreatedAt >= DateTime.UtcNow.AddDays(-30))
            .CountAsync();

        return recentScans == 0 ? 0 : Math.Clamp(40 + Math.Min(recentScans * 12, 60), 0, 100);
    }
}
