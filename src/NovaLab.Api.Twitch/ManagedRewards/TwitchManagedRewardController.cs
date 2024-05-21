// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Services.Twitch.TwitchTokens;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;

namespace NovaLab.Api.Twitch.ManagedRewards;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/{userId}/twitch/managed-rewards/")]
public class TwitchManagedRewardController(
    TwitchAPI twitchApi,
    ApplicationDbContext dbContext,
    TwitchTokensManager twitchTokensService,
    ILogger logger) : AbstractBaseController{

    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [SwaggerOperation(OperationId = "GetManagedRewards")]
    public async Task<IActionResult> GetManagedRewards(
        [FromRoute] string userId, 
        [FromQuery(Name = "limit")] uint? limit = null ) {

        IQueryable<TwitchManagedReward> query = dbContext.TwitchManagedRewards.Where(reward => reward.User.Id == userId).AsQueryable();

        TwitchManagedReward[] rewards = limit == null 
            ? await query.ToArrayAsync()
            : await query.Take((int)limit).ToArrayAsync();

        return Success(rewards);
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // POST Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType<ApiResult<TwitchManagedReward>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult<TwitchManagedReward>>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "PostManagedReward")]
    public async Task<IActionResult> PostManagedReward(
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
            return Success(output.Entity);
        }
        catch (Exception e) {
            logger.Warning(e, "Reward could not be created");
            return FailureServer(msg:"Reward could not be created");
        }
    }
}