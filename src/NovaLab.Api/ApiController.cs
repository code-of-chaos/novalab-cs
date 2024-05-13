// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Services.Twitch.TwitchTokens;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;

namespace NovaLab.Api;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[ApiController]
// [Authorize]
[Route("api/{userId}/twitch/redemptions/git-commit-message")]
public class ApiRootController(TwitchAPI twitchApi, ApplicationDbContext dbContext, TwitchTokensManager twitchTokensService) : Controller {
    
    [HttpGet]
    public async Task<ActionResult<ApiResultDto<TwitchManagedReward>>> Get([FromRoute] string userId) {
        TwitchManagedReward? redemption =await dbContext.TwitchManagedRewards.FirstOrDefaultAsync( o => o.User.Id == userId);
        return redemption is null
            ? new JsonResult(ApiResultDto<TwitchManagedReward>.Empty())
            : new JsonResult(ApiResultDto<TwitchManagedReward>.Successful(redemption));
    }

    [HttpPost]
    public async Task<ActionResult<TwitchManagedReward>> Post([FromRoute] string userId, CreateCustomRewardsRequest customRewardsRequest) {
        ApplicationUser? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return new JsonResult(ApiResultDto<TwitchManagedReward>.Empty());
        
        string? accessToken = await twitchTokensService.GetAccessTokenOrRefreshAsync(user);
        if (accessToken is null) return new JsonResult(ApiResultDto<TwitchManagedReward>.Empty());
        
        CreateCustomRewardsResponse? result = await twitchApi.Helix.ChannelPoints.CreateCustomRewardsAsync(
            user.TwitchBroadcasterId,
            customRewardsRequest,
            accessToken
        );
        
        if (result is null) return new JsonResult(ApiResultDto<TwitchManagedReward>.Empty());
        CustomReward? customReward = result.Data.First();
        var reward = new TwitchManagedReward {
            User = user,
            RewardId = customReward.Id,
            Title = customReward.Title,
            PointsCost = 0,
            HasPrompt = customReward.Prompt.IsNullOrEmpty()
        };
        
        EntityEntry<TwitchManagedReward> output =await dbContext.TwitchManagedRewards.AddAsync(reward);
        await dbContext.SaveChangesAsync();
        return new JsonResult(ApiResultDto<TwitchManagedReward>.Successful(output.Entity));
    }
}