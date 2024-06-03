// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Data.Models.Twitch.Followers;

namespace NovaLab.Api.Twitch.Followers.NewFollower;

using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Services.Twitch.Hubs;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/followers/new-follower")]
public class TwitchNewFollowerController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    IHubContext<TwitchHub> hubContext
    
    ) : AbstractBaseController(contextFactory){
    
    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<ApiResult<NewFollowerDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "GetNewFollowers")]
    public async Task<IActionResult> GetFollowerGoals(
        [FromQuery(Name = "user-id")] string? userId = null, 
        [FromQuery(Name = "goal-id")] Guid? goalId = null, 
        [FromQuery(Name = "from-date")] DateTime? fromDate = null, 
        [FromQuery(Name = "limit")] int? limit = null
    ) {
        await using NovaLabDbContext dbContext = await NovalabDb;
        try {
            IQueryable<TwitchNewFollower> query = dbContext.TwitchNewFollowers
                .Include(nf => nf.Goal)
                .ConditionalWhere(!string.IsNullOrEmpty(userId), nf => nf.Goal.User.TwitchBroadcasterId == userId )
                .ConditionalWhere(goalId is not null, nf => nf.Goal.Id == goalId)
                .ConditionalWhere(fromDate is not null, nf => nf.TimeStamp >= fromDate)
                .ConditionalTake(limit is not null, limit ?? 0 )
                .AsQueryable();

            TwitchNewFollower[] result = await query.ToArrayAsync();
            return Success(result.Select(NewFollowerDto.FromDbObject).ToArray());
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
    [ProducesResponseType<ApiResult<NewFollowerDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "PostNewFollower")]
    public async Task<IActionResult> PostNewFollower(
        [FromBody] PostNewFollowerDto postNewFollowerDto
    ) {
        await using NovaLabDbContext dbContext = await NovalabDb;
        
        try {
            
            NovaLabUser user = await dbContext.Users
                .Include(u => u.TwitchFollowerGoal)
                .FirstAsync(u => u.TwitchBroadcasterId == postNewFollowerDto.TwitchBoardcasterId);
            
            if (user is not { TwitchFollowerGoal: {} twitchFollowerGoal }) {
                logger.Warning("User not found"); // TODO add more logs
                return FailureClient();
            }
            
            bool followerAlreadyFollowed = await dbContext.TwitchNewFollowers
                .Include(nf => nf.Goal)
                .AnyAsync(nf => nf.Goal.Id == twitchFollowerGoal.Id && nf.FollowerTwitchUserId == postNewFollowerDto.FollowerTwitchUserId);
            if (followerAlreadyFollowed) {
                logger.Information("User has already followed the broadcaster before"); // TODO add more logs
                return FailureClient(msg:"User has already followed the broadcaster before");
            }
            
            var newFollower = new TwitchNewFollower {
                Goal = twitchFollowerGoal,
                TimeStamp = DateTime.Now,
                FollowerTwitchUserId = postNewFollowerDto.FollowerTwitchUserId
            };
            
            await dbContext.TwitchNewFollowers.AddAsync(newFollower);
            await dbContext.SaveChangesAsync();
            
            // send the client that this is to be updated
            await hubContext.SendNewTwitchFollower(user.Id);
            logger.Information("Sent to client");
            return Success(NewFollowerDto.FromDbObject(newFollower));
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }
    
}
