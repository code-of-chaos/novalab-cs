// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Authorization;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace NovaLab.WasmClient.Services;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record struct TryReturnValue<T>(bool Result, T Value) {
    public static TryReturnValue<T> Success(T value) => new(true, value);
    public static TryReturnValue<T> Failure(T value) => new(false, value);
}

public class NovaLabUserService(AuthenticationStateProvider authenticationStateProvider, ILogger logger) {
   
    public async Task<TryReturnValue<Guid>> GetUserIdAsync() {
        AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal claims = authState.User;
        // Get the user id claim
        string? userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is not null && Guid.TryParse(userIdClaim, out Guid userId)) return TryReturnValue<Guid>.Success(userId);
        
        logger.Warning("UserId could not be parsed");
        return TryReturnValue<Guid>.Failure(Guid.Empty);

    }
    
    public static bool Try<T>(TryReturnValue<T> result, [NotNullWhen(true)] out T? value) {
        value = default;
        if (result.Result) value = result.Value;
        return result.Result;
    }
}
