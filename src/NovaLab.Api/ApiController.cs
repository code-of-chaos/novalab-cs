// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NovaLab.Data;
using NovaLab.Data.Data.Twitch.Redemptions;
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
public class ApiRootController(TwitchAPI twitchApi, ILogger<ApiRootController> logger, ApplicationDbContext dbContext) : Controller {
    
    [HttpGet]
    public async Task<ActionResult<TwitchManagedReward>> Get([FromRoute] string userId) {
        TwitchManagedReward? redemption =await dbContext.TwitchManagedRewards.FirstOrDefaultAsync( o => o.User.Id == userId);
        return redemption is null
            ? new JsonResult(ApiResultDto<TwitchManagedReward>.Empty())
            : new JsonResult(ApiResultDto<TwitchManagedReward>.Successful());
    }

    [HttpPost]
    public async Task<ActionResult<TwitchManagedReward>> Post([FromRoute] string userId) {
        ApplicationUser? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return new JsonResult(ApiResultDto<TwitchManagedReward>.Empty());

        IQueryable<IdentityUserToken<string>> userTokens = dbContext.UserTokens.Where(ut => ut.UserId == user.Id && ut.LoginProvider == "Twitch" && ut.Name == "access_token");
        IdentityUserToken<string> token = await userTokens.FirstAsync();
        
        var rewardsRequest = new CreateCustomRewardsRequest {
            Title = "NovaLab Custom Reward",
            Prompt = null,
            Cost = 100,
            IsEnabled = false,
            BackgroundColor = null,
            IsUserInputRequired = false,
            IsMaxPerStreamEnabled = false,
            MaxPerStream = null,
            IsMaxPerUserPerStreamEnabled = false,
            MaxPerUserPerStream = null,
            IsGlobalCooldownEnabled = false,
            GlobalCooldownSeconds = null,
            ShouldRedemptionsSkipRequestQueue = false
        };

        CreateCustomRewardsResponse? result = await twitchApi.Helix.ChannelPoints.CreateCustomRewardsAsync(
            user.TwitchBroadcasterId,
            rewardsRequest,
            token.Value
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