// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.API;
using CodeOfChaos.AspNetCore.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.API.Models.Account;
using NovaLab.Server.Database;
using NovaLab.Server.Services.Jwt;
using Serilog;
using System.Net;

namespace NovaLab.Server.API.Controllers.Account;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ApiController]
[Route("api/account/tokens")]
public class JwtTokenController(
    IDbContextFactory<NovaLabDbContext> contextFactory,
    NovaLabJwtService jwtService,
    ILogger logger
) : BaseController(contextFactory) {

    [AllowAnonymous]
    [HttpGet("/refresh")]
    [ProducesResponse<IApiResult<RefreshTokenResponseDto>>(HttpStatusCode.OK)]
    [ProducesResponse<IApiResult>(HttpStatusCode.BadRequest)]
    public IActionResult RefreshToken(
        [FromQuery(Name = "refreshToken")] string oldRefreshToken
    ) {
        if (!jwtService.TryValidateToken(oldRefreshToken, out Guid userId, out _)) return FailureClient();

        (string accessToken, DateTime accessExpiresAt) = jwtService.GenerateAccessToken(userId);
        (string refreshToken, DateTime refreshExpiresAt) = jwtService.GenerateRefreshToken(userId);
        
        return Success(new RefreshTokenResponseDto(
            accessToken,
            refreshToken,
            accessExpiresAt,
            refreshExpiresAt
        ));
    }

    [Authorize]
    [HttpGet("/check")]
    public IActionResult Check() {
        logger.Warning("{u}", User.Identity?.Name ?? "UNDEFINED");
        return Success();
    }
}
