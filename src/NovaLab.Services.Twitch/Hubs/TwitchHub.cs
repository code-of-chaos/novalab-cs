// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.SignalR;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Hosted;
using Serilog;

namespace NovaLab.Services.Twitch.Hubs;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TwitchHub(IUserConnectionManager userConnectionManager, ILogger logger) : Hub {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public override async Task OnConnectedAsync() {
        if (Context.UserIdentifier is { } identityUserId // checks for not null as well, because awesome object pattern
            && !userConnectionManager.TryStoreUserConnection(identityUserId, Context.ConnectionId)) {
            logger.Warning("Identity {identity} could not be added to connection manager", identityUserId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        if (Context.UserIdentifier is { } identityUserId 
            && !userConnectionManager.TryRemoveUserConnection(identityUserId)) {
            logger.Warning("Identity {identity} could not be removed from connection manager", identityUserId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Custom events
    // -----------------------------------------------------------------------------------------------------------------
    public async Task SendNewManagedRewardRedemption(string userId, TwitchManagedRewardRedemption newRecord) {
        await Clients.User(userId).SendAsync(TwitchHubMethods.NewManagedRewardRedemption, newRecord);
    }
}