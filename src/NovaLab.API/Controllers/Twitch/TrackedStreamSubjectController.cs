// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using CodeOfChaos.AspNetCore.Contracts;
using ISOLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NovaLab.API.Services.Twitch;
using NovaLab.Lib.Twitch;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Models.Twitch;
using NovaLab.Server.Data.Shared.Models.Twitch;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;

namespace NovaLab.API.Controllers.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/tracked-stream-subject")]
public class TrackedStreamSubjectController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    TwitchAPI twitchApi,
    TwitchTokensManager twitchTokens,
    TwitchGameTitleToIdCacheService twitchCategoryCache
    ) : BaseController(contextFactory) {

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task<TrackedStreamSubject?> CreateNewTrackedStreamSubjectAsync(NovaLabDbContext dbContext, TrackedStreamSubjectDtoPost dtoPost) {
        if (await dbContext.Users.FirstOrDefaultAsync(novaLabUser => novaLabUser.Id == dtoPost.NovaLabUserId) is not {} user) {
            return null;
        }
        
        // Get the Twitch Category id when defined,
        //      If the category wasn't defined twitch will use the same one as the current one.
        string? twitchGameId = null;
        if (dtoPost.TwitchGameTitleName is not null) {
            twitchGameId = (await twitchCategoryCache.GetCategoryIdAsync(dtoPost.TwitchGameTitleName))?.TwitchTitleId;
        }
        
        return new TrackedStreamSubject {
            Id = default,
            User = user,
            TwitchGameId = twitchGameId,
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
    [SwaggerOperation(OperationId = nameof(GetTrackedStreamSubjects))]
    public async Task<IActionResult> GetTrackedStreamSubjects(
        [FromQuery(Name = "user-id")] Guid? userId = null
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            IQueryable<TrackedStreamSubject> subjects = dbContext.TrackedStreamSubjects
                .Include(subject => subject.User)
                .ConditionalWhere(userId is not null, subject => subject.User.Id == userId);

            TrackedStreamSubject[] result = await subjects.ToArrayAsync();
            
            return Success(result.Select(subject => subject.ToDto()).ToArray());
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
    [ProducesResponseType<IApiResult<TrackedStreamSubjectDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = nameof(PostTrackedStreamSubject))]
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
            logger.Warning(ex, "Unexpected error");
            return FailureServer();
        }
    }
    
    [HttpPost("select")]
    [ProducesResponseType<IApiResult<bool>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = nameof(SelectTrackedStreamSubject))]
    public async Task<IActionResult> SelectTrackedStreamSubject(
        [FromQuery(Name="user-id")] Guid userId,
        [FromQuery(Name="subject-id")] Guid subjectId
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TrackedStreamSubject? result = await dbContext.TrackedStreamSubjects
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
            
            return Success(result.ToDto());
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }
}
