# FinShield Module Integration Guide

## Overview

The FinShield module is a comprehensive financial fraud protection system that integrates multiple APIs for URL scanning, payment page analysis, and transaction monitoring.

## Architecture

### Services

#### 1. **FinShieldService**
- **Location**: `Services/Finance/FinShieldService.cs`
- **Purpose**: URL analysis for phishing and fraud detection
- **Methods**:
  - `ScanUrlAsync(string url)` - Analyzes URLs against multiple threat databases

**Features**:
- Google Safe Browsing API integration
- URLScan.io integration for URL reputation checking
- Local heuristic analysis (keyword detection, suspicious patterns)
- Cumulative risk scoring (0-100)
- Color-coded verdicts (🔴 Danger, 🟠 Caution, 🟡 Warning, 🟢 Safe)

#### 2. **GroqVisionService**
- **Location**: `Services/AI/GroqVisionService.cs`
- **Purpose**: AI-powered image analysis for payment page verification
- **Methods**:
  - `AnalyzeImageAsync(string base64Image)` - Analyzes screenshots for phishing indicators

**Features**:
- Llava vision model via Groq API
- Detects UI inconsistencies, logo impersonation
- HTTPS indicator verification
- Font and spacing anomaly detection
- Risk scoring (0-100)

#### 3. **FinShieldModule**
- **Location**: `Services/Finance/FinShieldModule.cs`
- **Purpose**: Module interface implementation for the dashboard system
- **Properties**:
  - Icon: 💰
  - Route: `/finshield`
  - Order: 5 (display order in sidebar)
  - Primary Color: #059669 (teal)

## Configuration

### API Keys Setup

API keys should be stored in **`appsettings.Development.json`** (automatically excluded from git).

#### Development Environment (Local)

Create/Update `appsettings.Development.json`:

```json
{
  "Groq": {
    "ApiKey": "gsk_YOUR_ACTUAL_GROQ_KEY_HERE",
    "Model": "llama-3.3-70b-versatile",
    "VisionModel": "llava-1.5-7b-4096-preview"
  },
  "GoogleSafeBrowsing": {
    "ApiKey": "YOUR_ACTUAL_GOOGLE_SAFE_BROWSING_KEY_HERE"
  },
  "URLScan": {
    "ApiKey": "YOUR_ACTUAL_URLSCAN_IO_KEY_HERE"
  },
  "AbuseIPDB": {
    "ApiKey": "YOUR_ACTUAL_ABUSEIPDB_KEY_HERE"
  }
}
```

#### Production Environment

Use Environment Variables or Azure Key Vault:
```bash
export Groq__ApiKey="your_production_key"
export GoogleSafeBrowsing__ApiKey="your_production_key"
export URLScan__ApiKey="your_production_key"
```

Or configure in `appsettings.Production.json` (if using environment-specific files).

## Dependency Injection Setup

The module is automatically registered in `Program.cs`:

```csharp
// ── AI Services ──
builder.Services.AddSingleton<GroqService>();
builder.Services.AddSingleton<GroqVisionService>();

// ── FinShield Services (Finance) ──
builder.Services.AddHttpClient<FinShieldService>()
    .ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.Add("User-Agent", "BASTION-FinShield/1.0");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

// ── Module Registration ──
builder.Services.AddScoped<IModule, FinShieldModule>();
```

## Usage in Razor Components

### UrlScanner Component Example

```razor
@page "/finshield/url-scanner"
@inject FinShieldService FinShield

<div class="url-scanner">
    <input @bind="urlInput" type="text" placeholder="Enter URL to scan..." />
    <button @onclick="ScanUrl">Scan URL</button>
    
    @if (result != null)
    {
        <div class="result">
            <h3>Verdict: @result.Verdict</h3>
            <div class="score">Risk Score: @result.ScamProbabilityScore%</div>
            
            @foreach (var flag in result.Flags)
            {
                <p>⚠️ @flag</p>
            }
        </div>
    }
</div>

@code {
    private string urlInput = "";
    private ScamAnalysisResult? result;
    
    private async Task ScanUrl()
    {
        if (string.IsNullOrWhiteSpace(urlInput)) return;
        result = await FinShield.ScanUrlAsync(urlInput);
    }
}
```

### PaymentPageScanner Component Example

```razor
@page "/finshield/payment-scanner"
@inject GroqVisionService VisionService

<div class="payment-scanner">
    <InputFile OnChange="@OnFileSelected" accept="image/*" />
    
    @if (visionResult != null)
    {
        <div class="vision-report">
            <h3>Analysis Summary</h3>
            <p>@visionResult.Summary</p>
            
            <div class="risk-score">Risk Score: @visionResult.RiskScore%</div>
            
            <h4>Detected Anomalies:</h4>
            <ul>
                @foreach (var anomaly in visionResult.Anomalies)
                {
                    <li>@anomaly</li>
                }
            </ul>
        </div>
    }
</div>

@code {
    private VisionAnalysisResult? visionResult;
    
    private async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        var buffer = new byte[file.Size];
        await file.OpenReadStream().ReadAsync(buffer);
        var base64 = Convert.ToBase64String(buffer);
        
        visionResult = await VisionService.AnalyzeImageAsync(base64);
    }
}
```

## API Integration Details

### 1. Google Safe Browsing API

**Endpoint**: `https://safebrowsing.googleapis.com/v4/threatMatches:find`

**Required**: Google Cloud API Key with Safe Browsing API enabled

**Score Contribution**: +40 points if malicious

### 2. URLScan.io API

**Endpoint**: `https://urlscan.io/api/v1/scan/`

**Required**: URLScan.io API Key

**Score Contribution**: +35 points if suspicious

### 3. Groq Vision API

**Endpoint**: `https://api.groq.com/openai/v1/chat/completions`

**Model**: `llava-1.5-7b-4096-preview`

**Required**: Groq API Key (free tier available)

### 4. Local Heuristics

**Score Contribution**: Up to +30 points based on:
- Suspicious keywords (free, win, gift, claim, urgent)
- Suspicious TLDs (.tk, .ml, .ga)
- IP-based URLs
- Domain patterns

## Security Considerations

1. **API Keys**: Never commit actual API keys. Use `.gitignore` to exclude:
   - `appsettings.Development.json`
   - `appsettings.Production.json` (if stored locally)
   - `.env` files

2. **Rate Limiting**: Configure appropriate timeouts:
   - FinShieldService: 30 seconds (configured)
   - GroqVisionService: 60 seconds (2+ seconds per request)

3. **Error Handling**: Services gracefully fall back to local analysis if external APIs fail

4. **User Data**: Screenshots and URLs are processed but not stored by default

## Testing

### Mock Data

Services include mock implementations when API keys aren't configured (placeholder values).

To test locally without API keys:
1. appsettings.Development.json can contain placeholder values
2. Services will use mock analysis results
3. Perfect for UI/UX testing and development

### Integration Testing

```csharp
[TestMethod]
public async Task FinShieldService_SuspiciousUrl_ReturnsHighScore()
{
    var service = new FinShieldService(httpClient, config);
    var result = await service.ScanUrlAsync("http://free-gift-win.tk");
    
    Assert.IsTrue(result.ScamProbabilityScore > 70);
    Assert.AreEqual("🔴 Danger - High Risk", result.Verdict);
}
```

## Troubleshooting

### Issue: "API Key not configured"
**Solution**: Ensure `appsettings.Development.json` exists in the root directory with proper API keys

### Issue: Services not injected
**Solution**: Verify `Program.cs` includes proper service registration (see Dependency Injection Setup)

### Issue: Timeout errors
**Solution**: Increase timeout values in `Program.cs` HttpClient configuration if network is slow

## Performance Metrics

- **FinShieldService URL Scan**: ~1.5-2 seconds (with external API calls)
- **GroqVisionService Image Analysis**: ~2-3 seconds (AI processing)
- **Local Heuristics**: <100ms (instant)

## Future Enhancements

- [ ] Database caching for analyzed URLs
- [ ] Machine learning model for phishing detection
- [ ] Real-time threat feed integration (MISP, AlienVault OTX)
- [ ] Browser extension integration
- [ ] Mobile app support
- [ ] Webhook notifications for high-risk detections
- [ ] User feedback loop for improving accuracy

## References

- [Google Safe Browsing API](https://developers.google.com/safe-browsing)
- [URLScan.io Documentation](https://urlscan.io/docs/)
- [Groq API Documentation](https://console.groq.com/docs/)
- [BASTION Architecture](../README.md)
