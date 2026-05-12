using BASTION.Interfaces;

namespace BASTION.Services.Gamification;

/// <summary>
/// IModule implementation for HackerLab — security training academy module.
/// </summary>
public class HackerLabModule : IModule
{
    public string Name => "HackerLab";
    public string Icon => "🎮";
    public string RouteUrl => "/hackerlab";
    public string Description => "Security academy — 5 domains, mini-games, XP, badges, rank progression";
    public string PrimaryHex => "#7C3AED";
    public string CssClass => "module-hacker";
    public int Order => 6;

    public Task<int> GetScoreAsync(string userId)
    {
        // TODO: Compute HackerLab score based on XP, completed domains, and rank
        return Task.FromResult(0);
    }
}
