using BASTION.Data;
using BASTION.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BASTION.Services.Gamification;

/// <summary>
/// IModule implementation for HackerLab — security training academy module.
/// </summary>
public class HackerLabModule : IModule
{
    private readonly BastionDbContext _db;

    public HackerLabModule(BastionDbContext db) => _db = db;

    public string Name => "HackerLab";
    public string Icon => "bi-controller";
    public string RouteUrl => "/hackerlab";
    public string Description => "Security academy — 5 domains, mini-games, XP, badges, rank progression";
    public string PrimaryHex => "#7C3AED";
    public string CssClass => "module-hacker";
    public int Order => 6;

    public async Task<int> GetScoreAsync(string userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userId);
        if (user == null) return 0;

        var completions = await _db.UserLabCompletions
            .Where(c => c.Email == userId)
            .Select(c => c.LabId)
            .Distinct()
            .CountAsync();
        var badges = await _db.UserBadges
            .Where(b => b.Email == userId)
            .Select(b => b.BadgeName)
            .Distinct()
            .CountAsync();

        return Math.Clamp((user.CurrentXp / 10) + (completions * 8) + (badges * 6), 0, 100);
    }
}
