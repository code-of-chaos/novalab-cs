// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using CodeOfChaos.AspNetCore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.Lib.Twitch;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Shared.Models.Twitch;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using TwitchLib.Api;

namespace NovaLab.API.Controllers.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/game-title-id-cache")]
public class TwitchGameTitleToIdCacheController (
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    TwitchAPI twitchApi,
    TwitchTokensManager twitchTokens
    ) : BaseController(contextFactory) {

    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    // [HttpGet]
    // [ProducesResponseType<IApiResult<TrackedStreamSubjectDto>>((int)HttpStatusCode.OK)]
    // [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    // [SwaggerOperation(OperationId = nameof(GetCachedGameTitleId))]
    // public async Task<IActionResult> GetCachedGameTitleId() {
    //     
    // }
    // TODO
}
