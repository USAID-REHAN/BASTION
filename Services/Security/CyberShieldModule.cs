using BASTION.Interfaces;

namespace BASTION.Services.Security;

/// <summary>
/// IModule implementation for CyberShield — registers module identity and exposes GetScoreAsync.
/// </summary>
public class CyberShieldModule : IModule
{
    public string Name => "CyberShield";
    public string Icon => "🛡️";
    public string RouteUrl => "/cybershield";
    public string Description => "Device protection — PowerShell audit, IP checker, password breach, chatbot";
    public string PrimaryHex => "#0891B2";
    public string CssClass => "module-cyber";
    public int Order => 3;

    public Task<int> GetScoreAsync(string userId)
    {
        // TODO: Compute CyberShield protection score for this user
        return Task.FromResult(0);
    }
}
