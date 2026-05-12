namespace BASTION.Services.Security;

/// <summary>
/// SHA-1 hash + HIBP k-Anonymity typed HttpClient for password breach checking.
/// Only the first 5 characters of the SHA-1 hash are sent to the API.
/// </summary>
public class BreachCheckService
{
    private readonly HttpClient _httpClient;

    public BreachCheckService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // TODO: Implement SHA-1 hashing + HIBP k-Anonymity API calls
}
