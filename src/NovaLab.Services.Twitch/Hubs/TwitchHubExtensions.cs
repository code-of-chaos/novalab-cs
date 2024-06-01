// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.SignalR;


namespace NovaLab.Services.Twitch.Hubs;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TwitchHubExtensions {

    // -----------------------------------------------------------------------------------------------------------------
    // Custom events
    // -----------------------------------------------------------------------------------------------------------------
    public async static Task SendNewManagedRewardRedemption<T>(this IHubContext<TwitchHub> context, string userId, T newRecord) {
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

    public async static Task SendNewTwitchFollower(this IHubContext<TwitchHub> context, string userId) {
        await context.Clients
            .Groups($"user_{userId}")
            .SendAsync(TwitchHubMethods.NewTwitchFollower) // TODO maybe add more than just +1
            .ConfigureAwait(false);
    }












}