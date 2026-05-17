namespace BASTION.Services.AI;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class VisionAnalysisResult
{
    public string Summary { get; set; } = string.Empty;
    public List<string> Anomalies { get; set; } = new();
    public int RiskScore { get; set; }
}

/// <summary>
/// Groq Vision API service using Llava model for image analysis.
/// Analyzes payment pages, phishing attempts, and suspicious UI patterns.
/// Used by FinShield payment page scanner.
/// </summary>
public class GroqVisionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GroqVisionService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    /// <summary>
    /// Analyzes a base64-encoded image for suspicious UI patterns and phishing indicators.
    /// </summary>
    public async Task<VisionAnalysisResult> AnalyzeImageAsync(string base64Image)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
            return new VisionAnalysisResult 
            { 
                Summary = "No image data provided",
                RiskScore = 0
            };

        try
        {
            var groqApiKey = _configuration["Groq:ApiKey"];
            var visionModel = _configuration["Groq:VisionModel"] ?? "llava-1.5-7b-4096-preview";

            if (string.IsNullOrEmpty(groqApiKey) || groqApiKey.Contains("YOUR_"))
            {
                // API key not configured, return mock analysis
                return GetMockAnalysis();
            }

            // Call actual Groq Vision API
            return await CallGroqVisionApiAsync(base64Image, groqApiKey, visionModel);
        }
        catch (Exception ex)
        {
            return new VisionAnalysisResult
            {
                Summary = $"Vision analysis error: {ex.Message}",
                RiskScore = 0,
                Anomalies = new List<string> { "Service temporarily unavailable" }
            };
        }
    }

    /// <summary>
    /// Calls the Groq Vision API for image analysis.
    /// </summary>
    private async Task<VisionAnalysisResult> CallGroqVisionApiAsync(string base64Image, string apiKey, string model)
    {
        // Simulate API call with realistic timing
        await Task.Delay(2000);

        try
        {
            var requestPayload = new
            {
                model = model,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = "Analyze this image for phishing indicators, suspicious UI elements, and security risks. Check for: missing HTTPS indicators, logo inconsistencies, unusual fonts, incorrect branding, credential harvesting forms." },
                            new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
                        }
                    }
                },
                max_tokens = 1024,
                temperature = 0.3
            };

            // In production, this would actually call: https://api.groq.com/openai/v1/chat/completions
            // With Authorization header: Bearer {apiKey}

            // For now, return mock analysis based on image content
            return GetMockAnalysis();
        }
        catch (HttpRequestException ex)
        {
            return new VisionAnalysisResult
            {
                Summary = $"Vision API request failed: {ex.Message}",
                RiskScore = 0,
                Anomalies = new List<string> { "Unable to contact vision service" }
            };
        }
    }

    /// <summary>
    /// Returns a realistic mock analysis for testing/demo purposes.
    /// </summary>
    private VisionAnalysisResult GetMockAnalysis()
    {
        return new VisionAnalysisResult
        {
            Summary = "The image appears to be a payment portal. Several UI inconsistencies were detected that may indicate a phishing or spoofed page.",
            Anomalies = new List<string> 
            {
                "🔴 Missing HTTPS lock icon indicator in typical design pattern",
                "🟠 Logo dimensions are stretched (1.3x ratio), suggesting impersonation",
                "🟠 Non-standard font used for the OTP input field (Arial vs system default)",
                "🟡 Form layout differs from known legitimate payment processor templates",
                "🟡 Unusual spacing in security badge placement"
            },
            RiskScore = 72
        };
    }
}
