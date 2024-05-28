// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.Followers.FollowerGoal;

using Extensions;
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
[Route("api/twitch/followers/follower-goal")]
public class TwitchFollowerGoalController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger) : AbstractBaseController(contextFactory){
   
    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<ApiResult<FollowerGoalDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "GetFollowerGoals")]
    public async Task<IActionResult> GetFollowerGoals(
        [FromQuery(Name = "user-id")] string? userId = null, 
        [FromQuery(Name = "goal-ids")] Guid[]? goalIds = null, 
        [FromQuery(Name = "limit")] int? limit = null
    ) {
        await using NovaLabDbContext dbContext = await NovalabDb;

        try {
            IQueryable<TwitchFollowerGoal> query = dbContext.TwitchFollowerGoals
                .Include(fg => fg.User)
                .ConditionalWhere(userId is not null,fg => fg.User.Id == userId)
                .ConditionalWhere(goalIds is not null, fg => goalIds!.Contains(fg.Id))
                .ConditionalTake(goalIds is null && limit is not null, limit ?? 0) // Cannot take more than one id already
            ;
            
            TwitchFollowerGoal[] result = await query.ToArrayAsync();
            return Success(result
                .Select(FollowerGoalDto.FromDbObject)
                .ToArray()
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
    [ProducesResponseType<ApiResult<FollowerGoalDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "PostFollowerGoal")]
    public async Task<IActionResult> PostFollowerGoal(
        [FromBody] PostFollowerGoalDto postFollowerGoalDto
    ) {
        await using NovaLabDbContext dbContext = await NovalabDb;
        
        try {
            NovaLabUser? user = await dbContext.Users.FindAsync(postFollowerGoalDto.UserId);
            if (user is null) {
                return FailureClient(msg: "User could not found");
            }

            var followerGoal = new TwitchFollowerGoal {
                User = user,
                DailyGoal = postFollowerGoalDto.DailyGoal,
                Divider = postFollowerGoalDto.Divider,
                CustomCssStyling = postFollowerGoalDto.CustomCssStyling
            };
            
            await dbContext.TwitchFollowerGoals.AddAsync(followerGoal);
            await dbContext.SaveChangesAsync();

            return Success(FollowerGoalDto.FromDbObject(followerGoal));

        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }
}
