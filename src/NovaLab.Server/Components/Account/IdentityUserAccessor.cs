using Microsoft.AspNetCore.Identity;
using NovaLab.Server.Data.Models.Account;

namespace NovaLab.Server.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<NovaLabUser> userManager, IdentityRedirectManager redirectManager) {
    public async Task<NovaLabUser> GetRequiredUserAsync(HttpContext context) {
        NovaLabUser? user = await userManager.GetUserAsync(context.User);

        if (user is null) {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
