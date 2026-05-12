namespace BASTION.Services.Finance;

/// <summary>
/// Typed HttpClient for Google Safe Browsing + URLScan.io APIs.
/// </summary>
public class FinShieldService
{
    private readonly HttpClient _httpClient;

    public FinShieldService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // TODO: Implement Google Safe Browsing API calls
    // TODO: Implement URLScan.io API calls
}
