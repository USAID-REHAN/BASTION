using System.Text;
using System.Text.Json;

namespace BASTION.Services.AI;

/// <summary>
/// Typed HttpClient for Groq text API — used by CyberShield, LexGuard, FinShield, HackerLab.
/// Calls Groq's OpenAI-compatible endpoint with Llama 3.3 70B.
/// </summary>
public class GroqService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GroqService(IConfiguration configuration)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.groq.com/openai/v1/"),
            Timeout = TimeSpan.FromSeconds(120)
        };
        _apiKey = configuration["Groq:ApiKey"] ?? throw new InvalidOperationException("Groq:ApiKey not found in configuration.");
        _model = configuration["Groq:Model"] ?? "llama-3.3-70b-versatile";
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    /// <summary>
    /// Send a chat completion request to Groq API.
    /// Returns the assistant's text response.
    /// </summary>
    public async Task<string> ChatAsync(string systemPrompt, string userMessage, double temperature = 0.3)
    {
        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            temperature,
            max_tokens = 4096
        };

        return await SendPostRequestAsync(requestBody);
    }

    /// <summary>
    /// Send a chat completion request with full conversation history to Groq API.
    /// </summary>
    public async Task<string> ChatAsync(IEnumerable<GroqChatMessage> messages, double temperature = 0.3)
    {
        var requestBody = new
        {
            model = _model,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
            temperature,
            max_tokens = 4096
        };

        return await SendPostRequestAsync(requestBody);
    }

    private async Task<string> SendPostRequestAsync(object requestBody)
    {
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Groq API returned {response.StatusCode}: {errorBody}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return messageContent ?? string.Empty;
    }
}

public class GroqChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
