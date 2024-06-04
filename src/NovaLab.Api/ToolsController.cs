// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NovaLab.Api;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/tools")]
public class ToolsController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    ILogger logger
) : AbstractBaseController(contextFactory) {

    [HttpPost]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.OK)]
    [ProducesResponseType<ApiResult>((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(OperationId = "PostMigrate")]
    public async Task<IActionResult> PostMigrate() {
        
        await using NovaLabDbContext dbContext = await NovalabDb;
        await dbContext.Database.MigrateAsync();

        return Success();
    }
}
