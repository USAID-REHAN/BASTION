using Microsoft.JSInterop;

namespace BASTION.Services.Sound;

/// <summary>
/// Manages sound on/off state and triggers JS audio calls.
/// Only C# class that may call bastion-sound.js.
/// </summary>
public class SoundService
{
    private readonly IJSRuntime _js;
    private bool _isSoundEnabled = true;
    private bool _isInitialized;

    public event Action? OnSoundStateChanged;

    public bool IsSoundEnabled => _isSoundEnabled;

    public SoundService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;
        try
        {
            var sound = await _js.InvokeAsync<string>("bastionStorage.get", "bastion-sound-enabled");
            if (!string.IsNullOrEmpty(sound))
                _isSoundEnabled = sound == "true";
            _isInitialized = true;
        }
        catch { /* JS not ready yet */ }
    }

    public async Task ToggleSound()
    {
        _isSoundEnabled = !_isSoundEnabled;
        await _js.InvokeVoidAsync("bastionStorage.set", "bastion-sound-enabled", _isSoundEnabled.ToString().ToLower());
        OnSoundStateChanged?.Invoke();
    }

    public async Task SetSoundEnabled(bool enabled)
    {
        _isSoundEnabled = enabled;
        await _js.InvokeVoidAsync("bastionStorage.set", "bastion-sound-enabled", _isSoundEnabled.ToString().ToLower());
        OnSoundStateChanged?.Invoke();
    }

    public async Task PlayChime()
    {
        if (!_isSoundEnabled) return;
        try
        {
            await _js.InvokeVoidAsync("bastionSound.playChime");
        }
        catch { /* Audio may not be available */ }
    }

    public async Task PlayPreview()
    {
        try
        {
            await _js.InvokeVoidAsync("bastionSound.playChime");
        }
        catch { /* Audio may not be available */ }
    }
}
