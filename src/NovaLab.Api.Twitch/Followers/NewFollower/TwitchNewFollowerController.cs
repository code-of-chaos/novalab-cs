// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.Followers.NewFollower;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Followers;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/followers/new-follower")]
public class TwitchNewFollowerController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger) : AbstractBaseController(contextFactory){
    
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

            return Success(NewFollowerDto.FromDbObject(newFollower));
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }
    
}
