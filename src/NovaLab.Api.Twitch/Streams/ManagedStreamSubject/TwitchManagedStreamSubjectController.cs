// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Streams;
using NovaLab.Services.Twitch.Hubs;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NovaLab.Api.Twitch.Streams.ManagedStreamSubject;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/managed-stream-subject/")]
public class TwitchManagedStreamSubjectController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger,
    IHubContext<TwitchHub> hubContext
    
    ) : AbstractBaseController(contextFactory){
    
    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<ApiResult<TwitchManagedStreamSubjectDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "GetManagedStreamSubjects")]
    public async Task<IActionResult> GetManagedStreamSubjects(
        [FromQuery(Name = "user-id")] string userId, 
        [FromQuery(Name = "limit")] int? limit = null ) {

        await using NovaLabDbContext dbContext = await NovalabDb;
        
        IQueryable<TwitchManagedStreamSubject> query = dbContext
            // Through the rewards, select the redemptions
            .TwitchManagedStreamSubjects
            .Where(subject => subject.User.Id == userId)
            .ConditionalTake(limit > 0, limit ?? 0)
            .AsQueryable();

        TwitchManagedStreamSubject[] result = await query.ToArrayAsync();
        return !result.IsNullOrEmpty()
            ? Success(result.Select(TwitchManagedStreamSubjectDto.FromDbObject).ToArray())
            : FailureClient(msg:"No subejct could be found");
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // POST Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "PostManagedStreamSubjects")]
    public async Task<IActionResult> PostManagedStreamSubjects(
        [FromBody] PostTwitchManagedStreamSubjectDto subjectDto
    ) {
        await using NovaLabDbContext dbContext = await NovalabDb;
        
        try {
            NovaLabUser? user = await dbContext.Users.FindAsync(subjectDto.UserId);
            if (user is null) {
                logger.Warning("No User found by the Id {id}", subjectDto.UserId);
                return FailureClient();
            }
            
            var subject = new TwitchManagedStreamSubject {
                User = user,
                SelectionName = subjectDto.SelectionName,
                ObsSubjectTitle = subjectDto.ObsSubjectTitle,
                TwitchSubjectTitle = subjectDto.TwitchSubjectTitle
            };

            int countAlreadyDefinedSelectionNames = await dbContext.TwitchManagedStreamSubjects
                .Where(s => s.User == user && s.SelectionName == subject.SelectionName)
                .CountAsync();
            if (countAlreadyDefinedSelectionNames != 0) {
                logger.Warning("User {userId} already defined {name}", user.Id, subject.SelectionName);
                return FailureClient(msg:"SelectionName already used for this user");
            } 

            await dbContext.TwitchManagedStreamSubjects.AddAsync(subject);
            await dbContext.SaveChangesAsync();
            
            // send the client that this is to be updated
            await hubContext.SendNewTwitchManagedSubject(user.Id, TwitchManagedStreamSubjectDto.FromDbObject(subject));
            logger.Information("Sent to client");
            return Success();
        }
        catch (Exception e) {
            logger.Error(e, "Reward could not be created");
            return FailureServer(msg:"Reward could not be created");
        }
        
    }

    [HttpPost("select")]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "PostSelectManagedStreamSubject")]
    public async Task<IActionResult> PostSelectManagedStreamSubject(
        [FromQuery(Name="user-id")] string userId,
        [FromQuery(Name="subject-name")] string subjectName
    ) {
        await using NovaLabDbContext dbContext = await NovalabDb;
        try {
            NovaLabUser? user = await dbContext.Users
                // .Include(user => user.SelectedManagedStreamSubject)
                .FirstOrDefaultAsync(user => user.Id == userId);
            if (user is null) return FailureClient();

            TwitchManagedStreamSubject? subject = await dbContext.TwitchManagedStreamSubjects
                .FirstOrDefaultAsync(subject => subject.User == user && subject.SelectionName == subjectName);
            if (subject is null) return FailureClient();

            user.SelectedManagedStreamSubject = subject;
            
            // TODO Send update to twitch and receive the update at the client's side Page of the subject renderer
            
            await dbContext.SaveChangesAsync();
        
            return Success();
        } catch (Exception e) {
            logger.Error(e, "Reward could not be created");
            return FailureServer(msg:"Reward could not be created");
        }
    }
}
