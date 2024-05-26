// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.SignalR;
using NovaLab.Data.Data.Twitch.Redemptions;

namespace NovaLab.Services.Twitch.Hubs;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TwitchHubExtensions {

    // -----------------------------------------------------------------------------------------------------------------
    // Custom events
    // -----------------------------------------------------------------------------------------------------------------
    public async static Task SendNewManagedRewardRedemption(this IHubContext<TwitchHub> context, string userId, TwitchManagedRewardRedemption newRecord) {
        await context.Clients
            .Groups($"user_{userId}")
            .SendAsync(TwitchHubMethods.NewManagedRewardRedemption, newRecord)
            .ConfigureAwait(false);
    }
    
    
    public async static Task SendClearedManagedRewardRedemption(this IHubContext<TwitchHub> context, string userId, Guid rewardId) {
        await context.Clients
            .Groups($"user_{userId}")
            .SendAsync(TwitchHubMethods.ClearedManagedReward, rewardId)
            .ConfigureAwait(false);
    }
}