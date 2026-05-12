using BASTION.Interfaces;

namespace BASTION.Services.Settings;

/// <summary>
/// IModule implementation for Settings — personalisation module.
/// </summary>
public class SettingsModule : IModule
{
    public string Name => "Settings";
    public string Icon => "bi-gear-fill";
    public string RouteUrl => "/settings";
    public string Description => "Personalisation — Light/dark mode, color wheel, sound settings";
    public string PrimaryHex => "#475569";
    public string CssClass => "module-settings";
    public int Order => 99;

    public Task<int> GetScoreAsync(string userId)
    {
        return Task.FromResult(100);
    }
}
