using BASTION.Interfaces;

namespace BASTION.Services.Dashboard;

/// <summary>
/// IModule implementation for Homescreen — the integration hub.
/// </summary>
public class HomescreenModule : IModule
{
    public string Name => "Dashboard";
    public string Icon => "bi-grid-1x2-fill";
    public string RouteUrl => "/";
    public string Description => "Integration hub — Dashboard, scores, activity feed, navigation";
    public string PrimaryHex => "#1D4ED8";
    public string CssClass => "module-home";
    public int Order => 1;

    public Task<int> GetScoreAsync(string userId)
    {
        return Task.FromResult(0);
    }
}
