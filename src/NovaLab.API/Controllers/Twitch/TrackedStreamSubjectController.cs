// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using CodeOfChaos.AspNetCore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Models.Twitch;
using NovaLab.Server.Data.Shared;
using NovaLab.Server.Data.Shared.Models.Twitch;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NovaLab.API.Controllers.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/twitch/tracked-stream-subject")]
public class TrackedStreamSubjectController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger
    ) : BaseController(contextFactory) {

    // -----------------------------------------------------------------------------------------------------------------
    // GET Methods
    // -----------------------------------------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType<IApiResult<TrackedStreamSubjectDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(OperationId = "GetTrackedStreamSubjects")]
    public async Task<IActionResult> GetTrackedStreamSubjects(
        [FromQuery(Name = "user-id")] Guid? userId = null
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            IQueryable<TrackedStreamSubject> subjects = dbContext.TrackedStreamSubjects
                .ConditionalWhere(userId is not null, subject => subject.User.Id == userId);

            return Success(await subjects.ToArrayAsync());
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
    [ProducesResponseType<IApiResult<TrackedStreamSubjectDto>>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "PostTrackedStreamSubject")]
    public async Task<IActionResult> PostTrackedStreamSubject(
        [FromBody] TrackedStreamSubjectDtoPost dto
    ) {
        await using NovaLabDbContext dbContext = await DbContext;

        try {
            CreateFromDtoResponse<TrackedStreamSubject> result = await TrackedStreamSubject.TryCreateFromDtoAsync(dbContext, dto);
            if (result.IsFailure) return FailureClient();
            
            TrackedStreamSubject createdSubject = result.Subject!;
            dbContext.TrackedStreamSubjects.Add(createdSubject);
            await dbContext.SaveChangesAsync();
            
            return Success(createdSubject.ToDto());
        }
        catch (Exception ex) {
            logger.Warning(ex, "ERROR");
            return FailureServer();
        }
    }
}
