// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CodeOfChaos.AspNetCore.API;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class AbstractBaseController<TDbContext>(
    IDbContextFactory<TDbContext> contextFactory
) : Controller where TDbContext : DbContext {

    protected Task<TDbContext> DbContext => contextFactory.CreateDbContextAsync();
    
    // -----------------------------------------------------------------------------------------------------------------
    // Result Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected static IActionResult Success<T>(params T[] objects) {
        return new JsonResult(ApiResult<T>.Success(objects));
    }
    protected static IActionResult Success<T>(string? msg = null, params T[] objects) {
        return new JsonResult(ApiResult<T>.Success(null, msg, objects));
    }
    protected static IActionResult Success<T>(HttpStatusCode? status =null, string? msg = null, params T[] objects) {
        return new JsonResult(ApiResult<T>.Success(status, msg, objects));
    }
    
    // Only use these if no data has to be sent back to the client
    protected static IActionResult Success() {
        return new JsonResult(ApiResult.Success([]));
    }
    protected static IActionResult Success(string msg) {
        return new JsonResult(ApiResult.Success(null, msg, []));
    }
    protected static IActionResult Success(HttpStatusCode status, string? msg = null) {
        return new JsonResult(ApiResult.Success(status, msg, []));
    }

    protected static IActionResult FailureClient(HttpStatusCode? status =null, string? msg = null ) {
        return new JsonResult(ApiResult.FailureClient(status, msg));
    }
    protected static IActionResult FailureServer(HttpStatusCode? status =null, string? msg = null ) {
        return new JsonResult(ApiResult.FailureServer(status, msg));
    }
}