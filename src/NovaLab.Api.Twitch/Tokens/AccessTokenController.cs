// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovaLab.Data;
using NovaLab.Services.Twitch.TwitchTokens;
using Swashbuckle.AspNetCore.Annotations;

namespace NovaLab.Api.Twitch.Tokens;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[ApiController]
// [Authorize]
[Route("api/{userId}/twitch/tokens")]
public class AccessTokenController(TwitchTokensManager twitchTokensManager, UserManager<ApplicationUser> userManager) : AbstractBaseController {
    
    [HttpGet("refresh")]
    [SwaggerOperation(OperationId = "RefreshTokens")]
    public async Task<IActionResult> RefreshTokens([FromRoute] string userId) {
        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        if (user is null) 
            return FailureClient(msg: "User could not be retrieved");
        if (!await twitchTokensManager.RefreshAccessTokenAsync(user))
            return FailureServer(msg: "Token could not be refreshed");
        return Success(HttpStatusCode.ResetContent,"Token refreshed");
    }
}   