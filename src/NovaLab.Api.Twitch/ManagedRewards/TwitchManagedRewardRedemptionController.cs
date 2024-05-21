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
[Route("api/{userId}/twitch/managed-rewards-redemptions/")]
public class TwitchManagedRewardRedemptionController(
    NovaLabDbContext dbContext,
    ILogger logger,
    IHubContext<TwitchHub> hubContext,
    IUserConnectionManager userConnectionManager
    
    ) : AbstractBaseController{
    
    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<ApiResult<TwitchManagedRewardRedemption>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "GetRedemptions")]
    public async Task<IActionResult> GetRedemptions(
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
        return !result.IsNullOrEmpty()
            ? Success(result)
            : FailureClient(msg:"No rewards could be redeemed");
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // POST Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "PostRedemption")]
    public async Task<IActionResult> PostRedemption(
        [FromRoute] string userId,
        [FromBody] TwitchManagedRewardRedemptionDto rewardRedemption) {

        try {
            var redemption = new TwitchManagedRewardRedemption {
                TwitchManagedReward = await dbContext.TwitchManagedRewards.FirstAsync(reward => reward.RewardId == rewardRedemption.RewardId),
                Username = rewardRedemption.Username,
                Message = rewardRedemption.Message
            };

            await dbContext.TwitchManagedRewardRedemptions.AddAsync(redemption);
        
            await dbContext.SaveChangesAsync();
            
            // send the client that this is to be updated
            //  TODO eventually don't send to all clients, but only to the client which needs it.
            if ( userConnectionManager.TryGetConnectionId(userId, out string? connectionId)) {
                await hubContext.Clients.Client(connectionId)
                    .SendAsync(TwitchHubMethods.NewManagedRewardRedemption, redemption)
                    .ConfigureAwait(false);
                logger.Information("Sent to client");
                return Success();
            }

            logger.Warning("Could not send to client ");
            return Success();
        }
        catch (Exception e) {
            logger.Warning(e, "Reward could not be created");
            return FailureServer(msg:"Reward could not be created");
        }
        
    }
}