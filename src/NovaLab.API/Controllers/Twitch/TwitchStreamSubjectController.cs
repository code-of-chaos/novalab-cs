// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using CodeOfChaos.AspNetCore.Contracts;
using ISOLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.API.Models.Twitch;
using NovaLab.API.Services.Twitch;
using NovaLab.Twitch;
using NovaLab.Database;
using NovaLab.Database.Models.Twitch;
using NovaLab.Database.Models.Twitch.HelixApi;
using System.Net;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;

namespace NovaLab.API.Controllers.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/tracked-stream-subject")]
public class TwitchStreamSubjectController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    TwitchAPI twitchApi,
    TwitchTokensManager twitchTokens,
    TwitchGameTitleToIdCacheService twitchCategoryCache
    ) : BaseController(contextFactory) {

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task<TwitchStreamSubject?> CreateTwitchStreamSubjectAsync(
        NovaLabDbContext dbContext, TrackedStreamSubjectDtoPost dtoPost, Ulid? ulid = null
    ) {
        if (await dbContext.Users.FirstOrDefaultAsync(novaLabUser => novaLabUser.Id == dtoPost.NovaLabUserId) is not {} user) {
            return null;
        }
        
        // Get the Twitch Category id when defined,
        //      If the category wasn't defined twitch will use the same one as the current one.
        string? twitchGameId = dtoPost.TwitchGameTitleName is not null
            ? (await twitchCategoryCache.GetCategoryByNameAsync(dtoPost.TwitchGameTitleName))?.TwitchTitleId
            : null;
        
        return new TwitchStreamSubject {
            Id = ulid ?? default,
            User = user,
            TwitchGameId = twitchGameId,
            TwitchBroadcastLanguage = dtoPost.TwitchBroadcastLanguage ?? Languages.EN.Alpha2,
            TwitchTitle = dtoPost.TwitchTitle,
            TwitchTags = dtoPost.TwitchTags
        };
    }

    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [ProducesResponse<IApiResult<TrackedStreamSubjectDto>>(HttpStatusCode.OK)]
    [ProducesResponse<IApiResult>(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetTwitchStreamSubjects(
        [FromQuery(Name = "user-id")] Guid? userId = null
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            IQueryable<TwitchStreamSubject> subjects = dbContext.ActiveTwitchStreamSubject
                .Include(subject => subject.User)
                .ConditionalWhere(userId is not null, subject => subject.User.Id == userId);

            TwitchStreamSubject[] result = await subjects.ToArrayAsync();
            TwitchGameTitleToIdCache?[] images = await Task.WhenAll(result
                .Select(item => item.TwitchGameId is not null 
                    ? twitchCategoryCache.GetCategoryByIdAsync(item.TwitchGameId)
                    : Task.FromResult<TwitchGameTitleToIdCache?>(null) )
                );
            
            return Success(result.Select((item, i) => TrackedStreamSubjectDto.FromDatabase(
                item,
                images[i]
            )).ToArray());
        }
        catch (Exception ex) {
            logger.Warning(ex, "Unexpected Error");
            return FailureServer();
        }
    }
    
    [HttpGet("/{subjectId}")]
    [ProducesResponse<IApiResult<TrackedStreamSubjectDto>>(HttpStatusCode.OK)]
    [ProducesResponse<IApiResult>(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetTwitchStreamSubject(
        [FromQuery(Name="user-id")] Guid userId,
        [FromQuery(Name="subject-id")] Ulid subjectId
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TwitchStreamSubject? result = await dbContext.ActiveTwitchStreamSubject
                .Include(subject => subject.User)
                .FirstOrDefaultAsync(subject => subject.Id == subjectId && subject.User.Id == userId);
            if (result is null) return FailureClient(msg:"No Tracked Subject found");

            
            return Success(TrackedStreamSubjectDto.FromDatabase(
                result,
                result.TwitchGameId is not null ? await twitchCategoryCache.GetCategoryByIdAsync(result.TwitchGameId) : null
            ));
        }
        catch (Exception ex) {
            logger.Warning(ex, "Unexpected Error");
            return FailureServer();
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // POST Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpPost]
    [ProducesResponse<IApiResult<TrackedStreamSubjectDto>>(HttpStatusCode.OK)]
    [ProducesResponse<ApiResult>(HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpsertTwitchStreamSubject(
        [FromBody] TrackedStreamSubjectDtoPost dto,
        [FromQuery] Ulid? subjectId = null
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TwitchStreamSubject? result = await CreateTwitchStreamSubjectAsync(dbContext, dto, subjectId);
            if (result is null) return FailureClient();

            if (subjectId is not null) {
                dbContext.TwitchStreamSubject.Update(result);
            } else {
                dbContext.TwitchStreamSubject.Add(result);
            }
            await dbContext.SaveChangesAsync();
            
            return Success(TrackedStreamSubjectDto.FromDatabase(
                result,
                result.TwitchGameId is not null ? await twitchCategoryCache.GetCategoryByIdAsync(result.TwitchGameId) : null
            ));
        }
        catch (Exception ex) {
            logger.Warning(ex, "Unexpected error");
            return FailureServer();
        }
    }
    
    [HttpPost("select")]
    [ProducesResponse<IApiResult<bool>>(HttpStatusCode.OK)]
    [ProducesResponse<ApiResult>(HttpStatusCode.BadRequest)]
    [ProducesResponse<ApiResult>(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SelectTwitchStreamSubject(
        [FromQuery(Name="user-id")] Guid userId,
        [FromQuery(Name="subject-id")] Ulid subjectId
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TwitchStreamSubject? result = await dbContext.ActiveTwitchStreamSubject
                .Include(subject => subject.User)
                .FirstOrDefaultAsync(subject => subject.Id == subjectId && subject.User.Id == userId);
            if (result is null) return FailureClient(msg:"No Tracked Subject found");
            if (result.User.TwitchBroadcasterId.IsNullOrEmpty()) return FailureServer(msg:"Broadcaster Id was not defined"); 
            
            await twitchApi.Helix.Channels.ModifyChannelInformationAsync(
                result.User.TwitchBroadcasterId,
                new ModifyChannelInformationRequest {
                    GameId = result.TwitchGameId,
                    BroadcasterLanguage = result.TwitchBroadcastLanguage,
                    Title = result.TwitchTitle,
                },
                await twitchTokens.GetAccessTokenOrRefreshAsync(userId)
            );
            
            return Success(TrackedStreamSubjectDto.FromDatabase(
                result,
                result.TwitchGameId is not null ? await twitchCategoryCache.GetCategoryByIdAsync(result.TwitchGameId) : null
            ));
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // DELETE Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpDelete]
    [ProducesResponse<IApiResult<bool>>(HttpStatusCode.OK)]
    [ProducesResponse<ApiResult>(HttpStatusCode.BadRequest)]
    [ProducesResponse<ApiResult>(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteTrackedStreamSubject(
        [FromQuery(Name = "subject-id")] Ulid subjectId
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TwitchStreamSubject? result = await dbContext.TwitchStreamSubject
                .FirstOrDefaultAsync(subject => subject.Id == subjectId);

            if (result is null) return FailureClient(msg:"No Tracked Subject found");
            result.SoftDelete();
            
            await dbContext.SaveChangesAsync();
            return Success(true);
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }
}
