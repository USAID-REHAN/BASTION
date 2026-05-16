using BASTION.Interfaces;

namespace BASTION.Services.Finance;

/// <summary>
/// IModule implementation for FinShield — financial fraud protection module.
/// </summary>
public class FinShieldModule : IModule
{
    public string Name => "FinShield";
    public string Icon => "bi-currency-exchange";
    public string RouteUrl => "/finshield";
    public string Description => "Fraud protection — URL scanner, message analyzer, scam feed, education";
    public string PrimaryHex => "#059669";
    public string CssClass => "module-fin";
    public int Order => 5;

    public Task<int> GetScoreAsync(string userId)
    {
        // TODO: Compute FinShield protection score for this user
        return Task.FromResult(0);
    }
}
