namespace BASTION.Services.AI;

/// <summary>
/// Typed HttpClient for Groq Vision API (Llava) — used by FinShield payment page scanner.
/// Lives in shared services but currently used only by FinShield.
/// </summary>
public class GroqVisionService
{
    private readonly HttpClient _httpClient;

    public GroqVisionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // TODO: Implement Groq Vision (Llava) API calls for image analysis
}
