// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using CodeOfChaos.AspNetCore.Contracts;
using ISOLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NovaLab.API.Models.Twitch;
using NovaLab.API.Services.Twitch;
using NovaLab.Lib.Twitch;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Models.Twitch;
using NovaLab.Server.Data.Models.Twitch.HelixApi;
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
    private async Task<TrackedStreamSubject?> CreateNewTrackedStreamSubjectAsync(NovaLabDbContext dbContext, TrackedStreamSubjectDtoPost dtoPost, Guid? guid = null) {
        if (await dbContext.Users.FirstOrDefaultAsync(novaLabUser => novaLabUser.Id == dtoPost.NovaLabUserId) is not {} user) {
            return null;
        }
        
        // Get the Twitch Category id when defined,
        //      If the category wasn't defined twitch will use the same one as the current one.
        string? twitchGameId = dtoPost.TwitchGameTitleName is not null
            ? (await twitchCategoryCache.GetCategoryByNameAsync(dtoPost.TwitchGameTitleName))?.TwitchTitleId
            : null;
        
        return new TrackedStreamSubject {
            Id = guid ?? default,
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
    [HttpGet("all")]
    [ProducesResponse<IApiResult<TrackedStreamSubjectDto>>(HttpStatusCode.OK)]
    [ProducesResponse<IApiResult>(HttpStatusCode.InternalServerError)]
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
            TwitchGameTitleToIdCache?[] images = await Task.WhenAll(result
                .Select(item => item.TwitchGameId is not null 
                    ? twitchCategoryCache.GetCategoryByIdAsync(item.TwitchGameId)
                    : Task.FromResult<TwitchGameTitleToIdCache?>(null) )
                );
            
            return Success(result.Select((item, i) => TrackedStreamSubjectDto.FromDto(
                item,
                images[i]
            )).ToArray());
        }
        catch (Exception ex) {
            logger.Warning(ex, "Unexpected Error");
            return FailureServer();
        }
    }
    
    [HttpGet]
    [ProducesResponse<IApiResult<TrackedStreamSubjectDto>>(HttpStatusCode.OK)]
    [ProducesResponse<IApiResult>(HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = nameof(GetTrackedStreamSubject))]
    public async Task<IActionResult> GetTrackedStreamSubject(
        [FromQuery(Name="user-id")] Guid userId,
        [FromQuery(Name="subject-id")] Guid subjectId
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TrackedStreamSubject? result = await dbContext.TrackedStreamSubjects
                .Include(subject => subject.User)
                .FirstOrDefaultAsync(subject => subject.Id == subjectId && subject.User.Id == userId);
            if (result is null) return FailureClient(msg:"No Tracked Subject found");

            
            return Success(TrackedStreamSubjectDto.FromDto(
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
    [SwaggerOperation(OperationId = nameof(UpsertTrackedStreamSubject))]
    public async Task<IActionResult> UpsertTrackedStreamSubject(
        [FromBody] TrackedStreamSubjectDtoPost dto,
        [FromQuery] Guid? subjectId = null
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TrackedStreamSubject? result = await CreateNewTrackedStreamSubjectAsync(dbContext, dto, subjectId);
            if (result is null) return FailureClient();

            if (subjectId is not null) {
                dbContext.TrackedStreamSubjects.Update(result);
            }
            else {
                dbContext.TrackedStreamSubjects.Add(result);
            }
            await dbContext.SaveChangesAsync();
            
            return Success(TrackedStreamSubjectDto.FromDto(
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
            
            return Success(TrackedStreamSubjectDto.FromDto(
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
    [SwaggerOperation(OperationId = nameof(DeleteTrackedStreamSubject))]
    public async Task<IActionResult> DeleteTrackedStreamSubject(
        [FromQuery(Name = "subject-id")] Guid subjectId
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            TrackedStreamSubject? result = await dbContext.TrackedStreamSubjects
                .FirstOrDefaultAsync(subject => subject.Id == subjectId);

            if (result is null) return FailureClient(msg:"No Tracked Subject found");
            
            dbContext.TrackedStreamSubjects.Remove(result);
            await dbContext.SaveChangesAsync();
            return Success(true);
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }
}
