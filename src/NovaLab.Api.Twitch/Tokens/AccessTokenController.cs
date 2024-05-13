// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Services.Twitch.TwitchTokens;

namespace NovaLab.Api.Twitch.Tokens;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[ApiController]
// [Authorize]
[Route("api/{userId}/twitch/tokens")]
public class AccessTokenController(TwitchTokensManager twitchTokensManager, UserManager<ApplicationUser> userManager) : Controller {
    
    [HttpGet("refresh")]
    public async Task<ActionResult<ApiResultDto<bool>>> RefreshTokens([FromRoute] string userId) {
        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        return new JsonResult(ApiResultDto<bool>.Successful(
            user is not null && await twitchTokensManager.RefreshAccessTokenAsync(user)
        ));
    }
}   