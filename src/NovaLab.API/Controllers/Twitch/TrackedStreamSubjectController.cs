// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using CodeOfChaos.AspNetCore.Contracts;
using ISOLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Models.Twitch;
using NovaLab.Server.Data.Models.Twitch.HelixApi;
using NovaLab.Server.Data.Shared;
using NovaLab.Server.Data.Shared.Models.Twitch;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Games;

namespace NovaLab.API.Controllers.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/tracked-stream-subject")]
public class TrackedStreamSubjectController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    TwitchAPI twitchApi
    ) : BaseController(contextFactory) {

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task<TrackedStreamSubject?> CreateNewTrackedStreamSubjectAsync(NovaLabDbContext dbContext, TrackedStreamSubjectDtoPost dtoPost) {
        if (await dbContext.Users.FirstOrDefaultAsync(novaLabUser => novaLabUser.Id == dtoPost.NovaLabUserId) is not {} user) {
            return null;
        }
        
        // TODO add logging
        string? twitchTitleId = null;
        if (dtoPost.TwitchGameTitleName.IsNotNullOrEmpty()) {
            TwitchGameTitleToIdCache? cacheHit = await dbContext.TwitchGameTitleToIdCache.FirstOrDefaultAsync(cache => cache.NovaLabName == dtoPost.TwitchGameTitleName);
            if (cacheHit is not null) {
                twitchTitleId = cacheHit.TwitchTitleId;
            } else {
                GetGamesResponse? response = await twitchApi.Helix.Games.GetGamesAsync(gameNames: [dtoPost.TwitchGameTitleName]);
                Game? game = response?.Games.FirstOrDefault();
                if (game != null) {
                    await dbContext.TwitchGameTitleToIdCache.AddAsync(new TwitchGameTitleToIdCache {
                        NovaLabName = dtoPost.TwitchGameTitleName!,
                        TwitchTitleId = game.Id,
                        TwitchTitleName = game.Name,
                        TwitchTitleBoxArtUrl = game.BoxArtUrl,
                        TwitchTitleIgdbId = null
                    });
                    twitchTitleId = game.Id;
                }
            }
        }
        
        return new TrackedStreamSubject {
            Id = default,
            User = user,
            TwitchGameId = twitchTitleId,
            TwitchBroadcastLanguage = dtoPost.TwitchBroadcastLanguage ?? Languages.EN.Alpha2,
            TwitchTitle = dtoPost.TwitchTitle,
            TwitchTags = dtoPost.TwitchTags,
            TrackedStreamSubjectComponent = null
        };
    }

    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<IApiResult<TrackedStreamSubjectDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "GetTrackedStreamSubjects")]
    public async Task<IActionResult> GetTrackedStreamSubjects(
        [FromQuery(Name = "user-id")] Guid? userId = null
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            IQueryable<TrackedStreamSubject> subjects = dbContext.TrackedStreamSubjects
                .ConditionalWhere(userId is not null, subject => subject.User.Id == userId);

            return Success(await subjects.ToArrayAsync());
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // POST Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType<IApiResult<TrackedStreamSubjectDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "PostTrackedStreamSubject")]
    public async Task<IActionResult> PostTrackedStreamSubject(
        [FromBody] TrackedStreamSubjectDtoPost dto
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TrackedStreamSubject? result = await CreateNewTrackedStreamSubjectAsync(dbContext, dto);
            if (result is null) return FailureClient();

            dbContext.TrackedStreamSubjects.Add(result);
            await dbContext.SaveChangesAsync();
            
            return Success(result.ToDto());
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }
}
