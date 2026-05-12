using BASTION.Interfaces;

namespace BASTION.Services.Auth;

/// <summary>
/// IModule implementation for GateKeeper — authentication module.
/// </summary>
public class GateKeeperModule : IModule
{
    public string Name => "GateKeeper";
    public string Icon => "bi-shield-lock-fill";
    public string RouteUrl => "/login";
    public string Description => "Authentication — Sign up, sign in, MFA, brute-force protection";
    public string PrimaryHex => "#DC2626";
    public string CssClass => "module-gate";
    public int Order => 0;

    public Task<int> GetScoreAsync(string userId)
    {
        return Task.FromResult(100);
    }
}
