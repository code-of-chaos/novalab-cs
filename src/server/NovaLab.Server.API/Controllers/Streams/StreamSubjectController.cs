// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using CodeOfChaos.AspNetCore.Contracts;
using ISOLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.API.Models.Streams;
using NovaLab.Server.Database;
using NovaLab.Server.Database.Models.Twitch;
using NovaLab.Server.Database.Models.Twitch.HelixApi;
using NovaLab.Server.Services.Twitch;
using System.Net;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;

namespace NovaLab.Server.API.Controllers.Streams;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/streams/subjects/{userId:guid}")]
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
        NovaLabDbContext dbContext, StreamSubjectDtoPost dtoPost, Guid userId, Ulid? ulid = null
    ) {
        if (await dbContext.Users.FirstOrDefaultAsync(novaLabUser => novaLabUser.Id == userId) is not {} user) {
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
    [HttpGet]
    [ProducesResponse<IApiResult<StreamSubjectDto>>(HttpStatusCode.OK)]
    [ProducesResponse<ApiResultInternalServerError>(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetTwitchStreamSubjects(
        Guid userId
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            IQueryable<TwitchStreamSubject> subjects = dbContext.ActiveTwitchStreamSubject
                .Include(subject => subject.User)
                .Where(subject => subject.User.Id == userId);

            TwitchStreamSubject[] result = await subjects.ToArrayAsync();
            TwitchGameTitleToIdCache?[] images = await Task.WhenAll(result
                .Select(item => item.TwitchGameId is not null 
                    ? twitchCategoryCache.GetCategoryByIdAsync(item.TwitchGameId)
                    : Task.FromResult<TwitchGameTitleToIdCache?>(null) )
                );
            
            return Success(result.Select((item, i) => StreamSubjectDto.FromDatabase(
                item,
                images[i]
            )).ToArray());
        }
        catch (Exception ex) {
            logger.Warning(ex, "Unexpected Error");
            return FailureServer();
        }
    }
    
    [HttpGet("{subjectId}")]
    [ProducesResponse<IApiResult<StreamSubjectDto>>(HttpStatusCode.OK)]
    [ProducesResponse<ApiResultInternalServerError>(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetTwitchStreamSubject(
        Guid userId,
        Ulid subjectId
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TwitchStreamSubject? result = await dbContext.ActiveTwitchStreamSubject
                .Include(subject => subject.User)
                .FirstOrDefaultAsync(subject => subject.Id == subjectId && subject.User.Id == userId);
            if (result is null) return FailureClient(msg:"No Tracked Subject found");

            
            return Success(StreamSubjectDto.FromDatabase(
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
    [ProducesResponse<IApiResult<StreamSubjectDto>>(HttpStatusCode.OK)]
    [ProducesResponse<ApiResultBadRequest>(HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpsertTwitchStreamSubject(
        Guid userId,
        [FromBody] StreamSubjectDtoPost dto,
        [FromQuery] Ulid? subjectId = null
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TwitchStreamSubject? result = await CreateTwitchStreamSubjectAsync(dbContext, dto, userId);
            if (result is null) return FailureClient();

            if (subjectId is not null) {
                dbContext.TwitchStreamSubject.Update(result);
            } else {
                dbContext.TwitchStreamSubject.Add(result);
            }
            await dbContext.SaveChangesAsync();
            
            return Success(StreamSubjectDto.FromDatabase(
                result,
                result.TwitchGameId is not null ? await twitchCategoryCache.GetCategoryByIdAsync(result.TwitchGameId) : null
            ));
        }
        catch (Exception ex) {
            logger.Warning(ex, "Unexpected error");
            return FailureServer();
        }
    }
    
    [HttpPost("{subjectId}/activate")]
    [ProducesResponse<IApiResult<bool>>(HttpStatusCode.OK)]
    [ProducesResponse<ApiResultBadRequest>(HttpStatusCode.BadRequest)]
    [ProducesResponse<ApiResultInternalServerError>(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ActiveStreamSubject(
        Ulid subjectId, Guid userId 
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
                    Title = result.TwitchTitle
                },
                await twitchTokens.GetAccessTokenOrRefreshAsync(result.User.Id)
            );
            
            return Success(StreamSubjectDto.FromDatabase(
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
    [HttpPost("{subjectId}/remove")]
    [ProducesResponse<IApiResult<bool>>(HttpStatusCode.OK)]
    [ProducesResponse<ApiResultBadRequest>(HttpStatusCode.BadRequest)]
    [ProducesResponse<ApiResultInternalServerError>(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteTrackedStreamSubject(
        Ulid subjectId, Guid userId 
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TwitchStreamSubject? result = await dbContext.TwitchStreamSubject
                .FirstOrDefaultAsync(subject => subject.Id == subjectId && subject.User.Id == userId);

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
