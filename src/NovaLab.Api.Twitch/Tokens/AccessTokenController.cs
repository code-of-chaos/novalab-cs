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
    TwitchTokensManager twitchTokensManager
) : AbstractBaseController(contextFactory) {
    
    [HttpGet("refresh")]
    [SwaggerOperation(OperationId = "RefreshTokens")]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> RefreshTokens([FromQuery] string userId) {
        if (!await twitchTokensManager.RefreshAccessTokenAsync(userId))
            return FailureServer(msg: "Token could not be refreshed");
        return Success(HttpStatusCode.ResetContent,"Token refreshed");
    }
}   