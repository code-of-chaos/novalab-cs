// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Services.Twitch.TwitchTokens;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;

namespace NovaLab.Api.Twitch.ManagedRewards;

using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.ChannelPoints.GetCustomReward;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/managed-rewards")]
public class TwitchManagedRewardController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    TwitchAPI twitchApi,
    TwitchTokensManager twitchTokensService,
    ILogger logger) : AbstractBaseController(contextFactory){

    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<ApiResult<TwitchManagedReward>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "GetManagedRewards")]
    public async Task<IActionResult> GetManagedRewards(
        [FromQuery(Name = "userId")] string? userId = null, 
        [FromQuery(Name = "limit")] uint? limit = null ) {
        await using NovaLabDbContext dbContext = await NovalabDb;

        try {
            IQueryable<TwitchManagedReward> query = dbContext.TwitchManagedRewards
                .AsNoTracking()
                .Include(reward => reward.User)
                .AsQueryable();

            if (userId is not null) query = query.Where(reward => reward.User.Id == userId);
            if (limit is not null) query = query.Take((int)limit);
            Dictionary<NovaLabUser, string[]> rewards = await query
                .GroupBy(reward => reward.User)
                .ToDictionaryAsync(
                    grouping => grouping.Key, 
                    grouping => grouping.Select(reward => reward.RewardId).ToArray()
                );

            string[] accessTokens = await Task.WhenAll(rewards.Keys
                .Select(user => twitchTokensService.GetAccessTokenOrRefreshAsync(user.Id))
                .ToArray()
            );

            GetCustomRewardsResponse[] responses = await Task.WhenAll(rewards.Keys
                .Zip(accessTokens)
                .Select(item => twitchApi.Helix.ChannelPoints.GetCustomRewardAsync(item.First.TwitchBroadcasterId, accessToken: item.Second))
                .ToArray()
            );
  
            HashSet<string> responseIds = responses
                .SelectMany(d => d.Data.Select(r => r.Id))
                .ToHashSet();

            HashSet<string> validIds = rewards.Values
                .SelectMany(rewardId => rewardId)
                .Where(rewardId => responseIds.Contains(rewardId))
                .ToHashSet();
            
            return Success(
                await query
                    .Where(reward => validIds.Contains(reward.RewardId))
                    .ToArrayAsync()
            );
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
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "PostManagedReward")]
    public async Task<IActionResult> PostManagedReward(
        [FromQuery] string? userId, 
        [FromBody] PostManagedRewardDto postManagedRewardDto) {
        
        await using NovaLabDbContext dbContext = await NovalabDb;
        postManagedRewardDto.TwitchApiRequest.IsEnabled = true;
        
        try {
            NovaLabUser user = await dbContext.Users.FirstAsync(u => u.Id == userId);
            
            CreateCustomRewardsResponse result = await twitchApi.Helix.ChannelPoints.CreateCustomRewardsAsync(
                user.TwitchBroadcasterId,
                postManagedRewardDto.TwitchApiRequest,
                await twitchTokensService.GetAccessTokenOrRefreshAsync(user.Id)
            );
            
            await dbContext.TwitchManagedRewards.AddAsync(
                new TwitchManagedReward {
                    User = user,
                    RewardId = result.Data.First().Id,
                    OutputTemplatePerRedemption = postManagedRewardDto.OutputTemplatePerRedemption,
                    OutputTemplatePerReward = postManagedRewardDto.OutputTemplatePerReward
                }
            );
            
            await dbContext.SaveChangesAsync();
            return Success();
        }
        catch (Exception e) {
            logger.Warning(e, "Reward could not be created");
            return FailureServer(msg:"Reward could not be created");
        }
    }

    [HttpPost("clear")]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "PostNewLastCleared")]
    public async Task<IActionResult> PostNewLastCleared(
        [FromQuery] string managedRewardId
    ) {
        await using NovaLabDbContext dbContext = await NovalabDb;

        TwitchManagedReward? reward = await dbContext.TwitchManagedRewards
            .FirstOrDefaultAsync(reward => reward.RewardId == managedRewardId);

        if (reward is null) return FailureClient();

        reward.LastCleared = DateTime.Now;
        await dbContext.SaveChangesAsync();

        return Success();
    }
}