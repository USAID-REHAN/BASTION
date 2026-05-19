using Microsoft.JSInterop;

namespace BASTION.Services.Sound;

/// <summary>
/// Manages sound on/off state and triggers JS audio calls.
/// Only C# class that may call bastion-sound.js.
/// </summary>
public class SoundService
{
    private readonly IJSRuntime _js;
    private bool _isChimeEnabled = true;
    private bool _isBeepEnabled = true;
    private string _selectedBgm = "none";
    private bool _isInitialized;

    public event Action? OnSoundStateChanged;

    public bool IsChimeEnabled => _isChimeEnabled;
    public bool IsBeepEnabled => _isBeepEnabled;
    public bool IsSoundEnabled => _isChimeEnabled || _isBeepEnabled;
    public string SelectedBgm => _selectedBgm;

    public SoundService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;
        try
        {
            var chime = await _js.InvokeAsync<string>("bastionStorage.get", "bastion-chime-enabled");
            if (!string.IsNullOrEmpty(chime))
                _isChimeEnabled = chime == "true";

            var beep = await _js.InvokeAsync<string>("bastionStorage.get", "bastion-beep-enabled");
            if (!string.IsNullOrEmpty(beep))
                _isBeepEnabled = beep == "true";

            var bgm = await _js.InvokeAsync<string>("bastionStorage.get", "bastion-bgm");
            if (!string.IsNullOrEmpty(bgm))
                _selectedBgm = bgm;

            if (_selectedBgm != "none")
            {
                await _js.InvokeVoidAsync("bastionSound.playBgm", $"/audio/{_selectedBgm}.mpeg");
            }

            _isInitialized = true;
        }
        catch { /* JS not ready yet */ }
    }

    public async Task SetChimeEnabled(bool enabled)
    {
        _isChimeEnabled = enabled;
        await _js.InvokeVoidAsync("bastionStorage.set", "bastion-chime-enabled", _isChimeEnabled.ToString().ToLower());
        OnSoundStateChanged?.Invoke();
    }

    public async Task SetBeepEnabled(bool enabled)
    {
        _isBeepEnabled = enabled;
        await _js.InvokeVoidAsync("bastionStorage.set", "bastion-beep-enabled", _isBeepEnabled.ToString().ToLower());
        OnSoundStateChanged?.Invoke();
    }

    public async Task ToggleSound()
    {
        bool target = !(_isChimeEnabled || _isBeepEnabled);
        _isChimeEnabled = target;
        _isBeepEnabled = target;
        await _js.InvokeVoidAsync("bastionStorage.set", "bastion-chime-enabled", _isChimeEnabled.ToString().ToLower());
        await _js.InvokeVoidAsync("bastionStorage.set", "bastion-beep-enabled", _isBeepEnabled.ToString().ToLower());
        OnSoundStateChanged?.Invoke();
    }

    public async Task SetBgm(string bgm)
    {
        _selectedBgm = bgm;
        await _js.InvokeVoidAsync("bastionStorage.set", "bastion-bgm", _selectedBgm);
        if (_selectedBgm == "none")
        {
            await _js.InvokeVoidAsync("bastionSound.playBgm", "");
        }
        else
        {
            await _js.InvokeVoidAsync("bastionSound.playBgm", $"/audio/{_selectedBgm}.mpeg");
        }
        OnSoundStateChanged?.Invoke();
    }

    public async Task PlayChime()
    {
        if (!_isChimeEnabled) return;
        try
        {
            await _js.InvokeVoidAsync("bastionSound.playChime");
        }
        catch { /* Audio may not be available */ }
    }

    public async Task PlayBeep()
    {
        if (!_isBeepEnabled) return;
        try
        {
            await _js.InvokeVoidAsync("bastionSound.playBeep");
        }
        catch { /* Audio may not be available */ }
    }

    public async Task PlayPreviewChime()
    {
        try
        {
            await _js.InvokeVoidAsync("bastionSound.playChime");
        }
        catch { /* Audio may not be available */ }
    }

    public async Task PlayPreviewBeep()
    {
        try
        {
            await _js.InvokeVoidAsync("bastionSound.playBeep");
        }
        catch { /* Audio may not be available */ }
    }
}
