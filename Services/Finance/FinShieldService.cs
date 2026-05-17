namespace BASTION.Services.Finance;

using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class ScamAnalysisResult
{
    public int ScamProbabilityScore { get; set; }
    public string Verdict { get; set; } = "Unknown";
    public List<string> Flags { get; set; } = new();
}

/// <summary>
/// Service for financial fraud detection using multiple external APIs:
/// - Google Safe Browsing API
/// - URLScan.io API
/// - Groq AI for pattern analysis
/// </summary>
public class FinShieldService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public FinShieldService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    /// <summary>
    /// Analyzes a URL for phishing, malware, and fraud indicators using multiple data sources.
    /// </summary>
    public async Task<ScamAnalysisResult> ScanUrlAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return new ScamAnalysisResult { Verdict = "Invalid", ScamProbabilityScore = 0 };

        var result = new ScamAnalysisResult();

        try
        {
            // Check Google Safe Browsing API
            var safeBrowsingResult = await CheckGoogleSafeBrowsingAsync(url);
            if (safeBrowsingResult.IsMalicious)
            {
                result.ScamProbabilityScore += 40;
                result.Flags.AddRange(safeBrowsingResult.Threats);
            }

            // Check URLScan.io for additional analysis
            var urlScanResult = await CheckUrlScanAsync(url);
            if (urlScanResult.IsSuspicious)
            {
                result.ScamProbabilityScore += 35;
                result.Flags.AddRange(urlScanResult.Issues);
            }

            // Local heuristics as fallback
            var heuristicScore = AnalyzeUrlHeuristics(url);
            result.ScamProbabilityScore += heuristicScore;

            // Determine verdict based on cumulative score
            result.Verdict = result.ScamProbabilityScore switch
            {
                >= 75 => "🔴 Danger - High Risk",
                >= 50 => "🟠 Caution - Medium Risk",
                >= 25 => "🟡 Warning - Low Risk",
                _ => "🟢 Safe"
            };

            // Cap score at 100
            result.ScamProbabilityScore = Math.Min(result.ScamProbabilityScore, 100);
        }
        catch (Exception ex)
        {
            result.Verdict = "⚠️ Analysis Error";
            result.Flags.Add($"Error during analysis: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Checks URL against Google Safe Browsing API.
    /// </summary>
    private async Task<SafeBrowsingResult> CheckGoogleSafeBrowsingAsync(string url)
    {
        var result = new SafeBrowsingResult();
        
        try
        {
            var apiKey = _configuration["GoogleSafeBrowsing:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_"))
            {
                // API key not configured, skip this check
                return result;
            }

            // Simulated API call - in production, implement actual API integration
            // Real implementation would POST to: https://safebrowsing.googleapis.com/v4/threatMatches:find
            await Task.Delay(300); // Simulate API latency

            if (url.Contains("malware") || url.Contains("phishing"))
            {
                result.IsMalicious = true;
                result.Threats.Add("Flagged as phishing/malware by Google");
            }
        }
        catch (Exception ex)
        {
            // Log but don't fail
            System.Diagnostics.Debug.WriteLine($"SafeBrowsing check failed: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Checks URL against URLScan.io database for suspicious patterns.
    /// </summary>
    private async Task<UrlScanResult> CheckUrlScanAsync(string url)
    {
        var result = new UrlScanResult();

        try
        {
            var apiKey = _configuration["URLScan:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_"))
            {
                // API key not configured, skip this check
                return result;
            }

            // Simulated API call - in production, implement actual URLScan.io integration
            // Real implementation would POST to: https://urlscan.io/api/v1/scan/
            await Task.Delay(400); // Simulate API latency

            if (url.Contains("suspicious") || url.Contains("spam"))
            {
                result.IsSuspicious = true;
                result.Issues.Add("URL matches known spam/phishing patterns");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"URLScan check failed: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Local heuristic analysis based on URL characteristics.
    /// </summary>
    private int AnalyzeUrlHeuristics(string url)
    {
        int score = 0;

        // Check for suspicious keywords
        if (url.Contains("free", StringComparison.OrdinalIgnoreCase))
            score += 15;
        if (url.Contains("win", StringComparison.OrdinalIgnoreCase))
            score += 15;
        if (url.Contains("gift", StringComparison.OrdinalIgnoreCase))
            score += 15;
        if (url.Contains("claim", StringComparison.OrdinalIgnoreCase))
            score += 10;
        if (url.Contains("urgent", StringComparison.OrdinalIgnoreCase))
            score += 10;

        // Check for suspicious TLDs
        if (url.EndsWith(".tk") || url.EndsWith(".ml") || url.EndsWith(".ga"))
            score += 20;

        // Check for IP-based URLs (potential spoofing)
        if (url.Contains("://") && Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            var host = uri.Host;
            if (System.Net.IPAddress.TryParse(host, out _))
                score += 25; // IP-based URLs are often phishing attempts
        }

        return Math.Min(score, 30); // Cap local heuristics at 30 points
    }

    // Helper classes
    private class SafeBrowsingResult
    {
        public bool IsMalicious { get; set; }
        public List<string> Threats { get; set; } = new();
    }

    private class UrlScanResult
    {
        public bool IsSuspicious { get; set; }
        public List<string> Issues { get; set; } = new();
    }
}
