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
                .Include(reward => reward.User)
                .AsQueryable();

            if (userId is not null) query = query.Where(reward => reward.User.Id == userId);
            if (limit is not null) query = query.Take((int)limit);
            Dictionary<NovaLabUser, TwitchManagedReward[]> rewards = await query
                .GroupBy(reward => reward.User)
                .ToDictionaryAsync(grouping => grouping.Key, grouping => grouping.ToArray());

            // Then create a new list for redeemed rewards
            foreach (NovaLabUser user in rewards.Keys) {
                IEnumerable<string> responseIds = (await twitchApi.Helix.ChannelPoints.GetCustomRewardAsync(
                user.TwitchBroadcasterId,
                accessToken: await twitchTokensService.GetAccessTokenOrRefreshAsync(user)
                )).Data.Select(d => d.Id);

                rewards[user] = rewards[user].Where(r => responseIds.Contains(r.RewardId)).ToArray();
            }
            
            return Success(rewards.Values
                .SelectMany(r => r) // FLAT PACK THE VALUES
                .ToArray());
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
        [FromBody] CreateCustomRewardsRequest customRewardsRequest) {
        
        await using NovaLabDbContext dbContext = await NovalabDb;
        customRewardsRequest.IsEnabled = true;
        
        try {
            NovaLabUser user = await dbContext.Users.FirstAsync(u => u.Id == userId);
            
            CreateCustomRewardsResponse result = await twitchApi.Helix.ChannelPoints.CreateCustomRewardsAsync(
                user.TwitchBroadcasterId,
                customRewardsRequest,
                await twitchTokensService.GetAccessTokenOrRefreshAsync(user)
            );
            
            await dbContext.TwitchManagedRewards.AddAsync(
                new TwitchManagedReward {
                    User = user,
                    RewardId = result.Data.First().Id
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
}