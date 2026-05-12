using BASTION.Interfaces;

namespace BASTION.Services.Legal;

/// <summary>
/// IModule implementation for LexGuard — document protection module.
/// </summary>
public class LexGuardModule : IModule
{
    public string Name => "LexGuard";
    public string Icon => "📜";
    public string RouteUrl => "/lexguard";
    public string Description => "Document protection — AI contract analysis, clause highlights, PDF export";
    public string PrimaryHex => "#D97706";
    public string CssClass => "module-lex";
    public int Order => 4;

    public Task<int> GetScoreAsync(string userId)
    {
        // TODO: Compute LexGuard protection score for this user
        return Task.FromResult(0);
    }
}
