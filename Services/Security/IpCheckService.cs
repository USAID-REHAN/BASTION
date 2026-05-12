namespace BASTION.Services.Security;

/// <summary>
/// Typed HttpClient for AbuseIPDB API — IP reputation checking.
/// </summary>
public class IpCheckService
{
    private readonly HttpClient _httpClient;

    public IpCheckService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // TODO: Implement AbuseIPDB API calls — abuse score, geolocation, ISP, threat categories
}
