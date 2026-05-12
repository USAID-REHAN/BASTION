using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BASTION.Services.Auth;

/// <summary>
/// Custom AuthenticationStateProvider for BASTION.
/// Bridges AuthService with Blazor's AuthorizeView and cascading auth state.
/// </summary>
public class BastionAuthStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _authService;

    public BastionAuthStateProvider(AuthService authService)
    {
        _authService = authService;
        _authService.OnAuthStateChanged += () =>
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        };
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = _authService.GetClaimsPrincipal();
        return Task.FromResult(new AuthenticationState(principal));
    }
}
