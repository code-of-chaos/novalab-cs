// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.Tokens;

using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovaLab.Data;
using NovaLab.Services.Twitch.TwitchTokens;
using Swashbuckle.AspNetCore.Annotations;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/tokens")]
public class AccessTokenController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    TwitchTokensManager twitchTokensManager,
    UserManager<NovaLabUser> userManager
) : AbstractBaseController(contextFactory) {
    
    [HttpGet("refresh")]
    [SwaggerOperation(OperationId = "RefreshTokens")]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> RefreshTokens([FromQuery] string userId) {
        NovaLabUser? user = await userManager.FindByIdAsync(userId);
        if (user is null) 
            return FailureClient(msg: "User could not be retrieved");
        if (!await twitchTokensManager.RefreshAccessTokenAsync(user))
            return FailureServer(msg: "Token could not be refreshed");
        return Success(HttpStatusCode.ResetContent,"Token refreshed");
    }
}   