// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Authorization;
using Serilog;
using System.Security.Claims;

namespace NovaLab.WasmClient.Services;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabUserService(AuthenticationStateProvider authenticationStateProvider, ILogger logger) {
    public async Task<Guid?> GetUserIdAsync() {
        AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal claims = authState.User;
        // Get the user id claim
        string? userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is not null && Guid.TryParse(userIdClaim, out Guid userId)) return userId;
        
        logger.Warning("UserId could not be parsed");
        return null;

    }
}
