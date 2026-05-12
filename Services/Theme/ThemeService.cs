using Microsoft.JSInterop;

namespace BASTION.Services.Theme;

/// <summary>
/// Manages light/dark state, accent color, and CSS variable injection.
/// Publishes C# events — all subscribed components re-render reactively.
/// Reads/writes to localStorage for persistence across page reloads.
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _js;
    private bool _isDarkMode = true;
    private string _accentColor = "#1D4ED8";

    public event Action? OnThemeChanged;

    public bool IsDarkMode => _isDarkMode;
    public string AccentColor => _accentColor;

    public ThemeService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Reads persisted theme from localStorage and applies CSS variables.
    /// Safe to call multiple times — each call reads latest from storage.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            var darkMode = await _js.InvokeAsync<string>("bastionStorage.get", "bastion-dark-mode");
            if (!string.IsNullOrEmpty(darkMode))
                _isDarkMode = darkMode == "true";

            var accent = await _js.InvokeAsync<string>("bastionStorage.get", "bastion-accent-color");
            if (!string.IsNullOrEmpty(accent))
                _accentColor = accent;

            await ApplyTheme();
        }
        catch
        {
            // JS not ready yet on very first static render — safe to ignore.
            // Will be initialized on first user interaction.
        }
    }

    public async Task ToggleTheme()
    {
        _isDarkMode = !_isDarkMode;
        await SaveAndApply();
    }

    public async Task SetDarkMode(bool dark)
    {
        _isDarkMode = dark;
        await SaveAndApply();
    }

    public async Task SetAccentColor(string hex)
    {
        _accentColor = hex;
        await SaveAndApply();
    }

    /// <summary>
    /// Persists BOTH dark mode + accent color to localStorage, then applies CSS.
    /// </summary>
    private async Task SaveAndApply()
    {
        try
        {
            await _js.InvokeVoidAsync("bastionStorage.set", "bastion-dark-mode", _isDarkMode.ToString().ToLower());
            await _js.InvokeVoidAsync("bastionStorage.set", "bastion-accent-color", _accentColor);
            await ApplyTheme();
            OnThemeChanged?.Invoke();
        }
        catch
        {
            // Guard against JS interop failures during prerender
        }
    }

    private async Task ApplyTheme()
    {
        await _js.InvokeVoidAsync("bastionTheme.apply", _isDarkMode, _accentColor);
    }
}
