// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.RazorLib.Lib;

using Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class UserAccessor(AuthenticationStateProvider authenticationStateProvider, IServiceScopeFactory scopeFactory) {
    public async Task<NovaLabUser?> GetUserAsync() {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<NovaLabUser>>();
        
        AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        string userName = authState.User.Identity!.Name!;
        NovaLabUser? user = await userManager.FindByNameAsync(userName);
        return user!;
    }
}
