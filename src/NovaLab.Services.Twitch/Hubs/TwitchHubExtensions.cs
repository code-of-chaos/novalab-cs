// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.SignalR;


namespace NovaLab.Services.Twitch.Hubs;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TwitchHubExtensions {
    private async static Task SendToClientGroup<T>(IHubContext<TwitchHub> context, string userId, T data, string method) {
        await context.Clients
            .Groups($"user_{userId}")
            .SendAsync(method, data)
            .ConfigureAwait(false);
    }
    private async static Task SendToClientGroup(IHubContext<TwitchHub> context, string userId, string method) {
        await context.Clients
            .Groups($"user_{userId}")
            .SendAsync(method)
            .ConfigureAwait(false);
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Custom events
    // -----------------------------------------------------------------------------------------------------------------
    public async static Task SendNewManagedRewardRedemption<T>(this IHubContext<TwitchHub> context, string userId, T data) {
        await SendToClientGroup(context, userId, data, TwitchHubMethods.NewManagedRewardRedemption);
    }
    
    public async static Task SendClearedManagedRewardRedemption(this IHubContext<TwitchHub> context, string userId, Guid data) {
        await SendToClientGroup(context, userId, data, TwitchHubMethods.ClearedManagedReward);
    }

    public async static Task SendNewTwitchFollower(this IHubContext<TwitchHub> context, string userId) {
        await SendToClientGroup(context, userId, TwitchHubMethods.NewTwitchFollower);
    }

    public async static Task SendNewTwitchManagedSubject<T>(this IHubContext<TwitchHub> context, string userId, T data) {
        await SendToClientGroup(context, userId, data, TwitchHubMethods.NewTwitchManagedSubject);
    }

    public async static Task SendSelectedTwitchManagedSubject<T>(this IHubContext<TwitchHub> context, string userId, T data) {
        await SendToClientGroup(context, userId, data, TwitchHubMethods.SelectedTwitchManagedSubject);
    }












}