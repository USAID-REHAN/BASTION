namespace BASTION.Services.AI;

/// <summary>
/// Typed HttpClient for Groq text API — used by CyberShield, LexGuard, FinShield, HackerLab.
/// </summary>
public class GroqService
{
    private readonly HttpClient _httpClient;

    public GroqService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // TODO: Implement Groq Llama 3.3 70B text API calls
    // TODO: Shared by Cyber, Lex, Fin, Hacker modules
}
