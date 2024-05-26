// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Hosted;
using NovaLab.Services.Twitch.Hubs;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace NovaLab.Api.Twitch.ManagedRewards;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/managed-rewards-redemptions/")]
public class TwitchManagedRewardRedemptionController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    IHubContext<TwitchHub> hubContext,
    IUserConnectionManager userConnectionManager
    
    ) : AbstractBaseController(contextFactory){
    
    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<ApiResult<TwitchManagedRewardRedemption>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "GetRedemptions")]
    public async Task<IActionResult> GetRedemptions(
        [FromQuery(Name = "user-id")] string userId, 
        [FromQuery(Name = "reward-id")] Guid? rewardId = null,
        [FromQuery(Name = "after")] DateTime? after = null,
        [FromQuery(Name = "limit")] uint? limit = null ) {

        await using NovaLabDbContext dbContext = await NovalabDb;
        
        IQueryable<TwitchManagedRewardRedemption> query = dbContext
            .TwitchManagedRewardRedemptions
            .Include(redemption => redemption.TwitchManagedReward)
            .Where(redemption => redemption.TwitchManagedReward.User.Id == userId)
            .Where(redemption => redemption.TimeStamp >= redemption.TwitchManagedReward.LastCleared)
            .AsQueryable();

        if (rewardId is not null) query = query.Where(redemption => redemption.TwitchManagedReward.Id == rewardId);
        if (after is not null) query = query.Where(redemption => redemption.TimeStamp >= after);
        if (limit is not null) query = query.Take((int)limit);

        TwitchManagedRewardRedemption[] result = await query.ToArrayAsync();
        return !result.IsNullOrEmpty()
            ? Success(result)
            : FailureClient(msg:"No rewards could be redeemed");
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // POST Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "PostRedemption")]
    public async Task<IActionResult> PostRedemption(
        [FromBody] TwitchManagedRewardRedemptionDto rewardRedemption
        ) {
        await using NovaLabDbContext dbContext = await NovalabDb;
        
        try {
            TwitchManagedReward? reward = await dbContext.TwitchManagedRewards
                .Include(twitchManagedReward => twitchManagedReward.User)
                .FirstOrDefaultAsync(reward => reward.RewardId == rewardRedemption.RewardId);
            if (reward is null) {
                logger.Warning("Could not map reward to rewardId {i}", rewardRedemption.RewardId);
                return FailureClient();
            }
            
            var redemption = new TwitchManagedRewardRedemption {
                TwitchManagedReward = reward,
                Username = rewardRedemption.Username,
                Message = rewardRedemption.Message,
                TimeStamp = DateTime.Now
            };

            await dbContext.TwitchManagedRewardRedemptions.AddAsync(redemption);
            await dbContext.SaveChangesAsync();
            
            logger.Error("{@e}", userConnectionManager.Map);
            
            // send the client that this is to be updated
            await hubContext.SendNewManagedRewardRedemption(reward.User.Id, redemption);
            logger.Information("Sent to client");
            return Success();
        }
        catch (Exception e) {
            logger.Warning(e, "Reward could not be created");
            return FailureServer(msg:"Reward could not be created");
        }
        
    }
}