// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

namespace NovaLab.Api.Twitch.ManagedRewards.ManagedRewardRedemption;

using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Services.Twitch.Hubs;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/managed-rewards-redemptions/")]
public class TwitchManagedRewardRedemptionController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    IHubContext<TwitchHub> hubContext
    
    ) : AbstractBaseController(contextFactory){
    
    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<ApiResult<TwitchManagedRewardRedemptionDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "GetRedemptions")]
    public async Task<IActionResult> GetRedemptions(
        [FromQuery(Name = "user-id")] string userId, 
        [FromQuery(Name = "reward-id")] Guid? rewardId = null,
        [FromQuery(Name = "after")] DateTime? after = null,
        [FromQuery(Name = "limit")] int? limit = null ) {

        await using NovaLabDbContext dbContext = await NovalabDb;
        
        IQueryable<TwitchManagedRewardRedemption> query = dbContext
            // Through the rewards, select the redemptions
            .TwitchManagedRewards
            .Where(reward => reward.User.Id == userId)
            .ConditionalWhere(rewardId is not null, reward => reward.Id == rewardId)
            // Actually get all the redemptions from the reward
            .SelectMany(reward => reward.TwitchManagedRewardRedemptions)
            .Include(redemption => redemption.TwitchManagedReward)
            .Where(redemption => redemption.TimeStamp >= redemption.TwitchManagedReward.LastCleared)
            .ConditionalWhere(after is not null, redemption => redemption.TimeStamp >= after)
            .ConditionalTake(limit > 0, limit ?? 0)
            .AsQueryable();

        TwitchManagedRewardRedemption[] result = await query.ToArrayAsync();
        return !result.IsNullOrEmpty()
            ? Success(result.Select(TwitchManagedRewardRedemptionDto.FromDbObject).ToArray())
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
        [FromBody] PostTwitchManagedRewardRedemptionDto redemptionDto
    ) {
        await using NovaLabDbContext dbContext = await NovalabDb;
        
        try {
            TwitchManagedReward? reward = await dbContext.TwitchManagedRewards
                .Include(twitchManagedReward => twitchManagedReward.User)
                .FirstOrDefaultAsync(reward => reward.RewardId == redemptionDto.TwitchRewardId);
            if (reward is null) {
                logger.Warning("Could not map reward to rewardId {i}", redemptionDto.TwitchRewardId);
                return FailureClient();
            }
            
            var redemption = new TwitchManagedRewardRedemption {
                TwitchManagedReward = reward,
                Username = redemptionDto.UserName,
                Message = redemptionDto.Message,
                TimeStamp = DateTime.Now
            };

            await dbContext.TwitchManagedRewardRedemptions.AddAsync(redemption);
            await dbContext.SaveChangesAsync();
            
            // send the client that this is to be updated
            await hubContext.SendNewManagedRewardRedemption(reward.User.Id, TwitchManagedRewardRedemptionDto.FromDbObject(redemption));
            logger.Information("Sent to client");
            return Success();
        }
        catch (Exception e) {
            logger.Warning(e, "Reward could not be created");
            return FailureServer(msg:"Reward could not be created");
        }
        
    }
}