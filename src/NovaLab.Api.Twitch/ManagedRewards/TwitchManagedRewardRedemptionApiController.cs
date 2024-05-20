// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Services.Twitch.Hub;
using Serilog;

namespace NovaLab.Api.Twitch.ManagedRewards;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/{userId}/twitch/managed-rewards-redemptions/")]
public class TwitchManagedRewardRedemptionApiController(
    ApplicationDbContext dbContext,
    ILogger logger,
    IHubContext<TwitchHub> hubContext
    
    ) : Controller{
    
    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<ApiResultDto<TwitchManagedRewardRedemption>>> Get(
        [FromRoute] string userId, 
        [FromQuery(Name = "reward-id")] Guid? rewardId = null,
        [FromQuery(Name = "after")] DateTime? after = null,
        [FromQuery(Name = "limit")] uint? limit = null ) {

        IQueryable<TwitchManagedRewardRedemption> query = dbContext
            .TwitchManagedRewardRedemptions
            .Include(redemption => redemption.TwitchManagedReward)
            .Where(redemption => redemption.TwitchManagedReward.User.Id == userId)
            .AsQueryable();

        if (rewardId is not null) query = query.Where(redemption => redemption.TwitchManagedReward.Id == rewardId);
        if (after is not null) query = query.Where(redemption => redemption.TimeStamp >= after);
        if (limit is not null) query = query.Take((int)limit);

        TwitchManagedRewardRedemption[] result = await query.ToArrayAsync();
        return result.IsNullOrEmpty()
            ? new JsonResult(ApiResultDto<TwitchManagedRewardRedemption>.Empty())
            : new JsonResult(ApiResultDto<TwitchManagedRewardRedemption>.Successful(result));
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // POST Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpPost]
    public async Task<ActionResult<ApiResultDto<bool>>> Post(
        [FromRoute] string userId,
        [FromBody] TwitchManagedRewardRedemptionDto rewardRedemption) {
        
        var redemption = new TwitchManagedRewardRedemption {
            TwitchManagedReward = await dbContext.TwitchManagedRewards.FirstAsync(reward => reward.RewardId == rewardRedemption.RewardId),
            Username = rewardRedemption.Username,
            Message = rewardRedemption.Message
        };

        await dbContext.TwitchManagedRewardRedemptions.AddAsync(redemption);
        
        await dbContext.SaveChangesAsync();
        await hubContext.Clients.All
            .SendAsync(TwitchHubMethods.NewManagedRewardRedemption, redemption)
            .ConfigureAwait(false);
            
        return new JsonResult(ApiResultDto<bool>.Empty());
    }
}