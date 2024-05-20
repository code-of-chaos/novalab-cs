// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Services.Twitch.TwitchTokens;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;

namespace NovaLab.Api.Twitch.ManagedRewards;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/{userId}/twitch/managed-rewards/")]
public class TwitchManagedRewardApiController(
    TwitchAPI twitchApi,
    ApplicationDbContext dbContext,
    TwitchTokensManager twitchTokensService,
    ILogger logger) : Controller{

    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<ApiResultDto<TwitchManagedReward>>> Get(
        [FromRoute] string userId, 
        [FromQuery(Name = "limit")] uint? limit = null ) {

        IQueryable<TwitchManagedReward> query = dbContext.TwitchManagedRewards.Where(reward => reward.User.Id == userId).AsQueryable();

        TwitchManagedReward[] rewards = limit == null 
            ? await query.ToArrayAsync()
            : await query.Take((int)limit).ToArrayAsync();
        
        return rewards.IsNullOrEmpty()
            ? new JsonResult(ApiResultDto<TwitchManagedReward>.Empty())
            : new JsonResult(ApiResultDto<TwitchManagedReward>.Successful(rewards));
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // POST Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpPost]
    public async Task<ActionResult<TwitchManagedReward>> Post(
        [FromRoute] string userId, 
        [FromBody] CreateCustomRewardsRequest customRewardsRequest) {
        
        customRewardsRequest.IsEnabled = true;
        
        try {
            ApplicationUser user = await dbContext.Users.FirstAsync(u => u.Id == userId);
            
            CreateCustomRewardsResponse result = await twitchApi.Helix.ChannelPoints.CreateCustomRewardsAsync(
                user.TwitchBroadcasterId,
                customRewardsRequest,
                await twitchTokensService.GetAccessTokenOrRefreshAsync(user)
            );
            
            EntityEntry<TwitchManagedReward> output =await dbContext.TwitchManagedRewards.AddAsync(
                new TwitchManagedReward {
                    User = user,
                    RewardId = result.Data.First().Id
                }
            );
            
            await dbContext.SaveChangesAsync();
            return new JsonResult(ApiResultDto<TwitchManagedReward>.Successful(output.Entity));
        }
        catch (Exception e) {
            logger.Warning(e, "Reward could not be created");
            return new JsonResult(ApiResultDto<TwitchManagedReward>.Failure("Reward could not be created"));
        }
    }
}