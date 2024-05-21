// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.SignalR;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Hosted;

namespace NovaLab.Services.Twitch.Hubs;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchHub(IUserConnectionManager userConnectionManager) : Hub {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public override async Task OnConnectedAsync() {
        string? identityUserId = Context.UserIdentifier;
        if (identityUserId is not null) {
            userConnectionManager.StoreUserConnection(identityUserId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        string? identityUserId = Context.UserIdentifier;
        if (identityUserId is not null) {
            userConnectionManager.RemoveUserConnection(identityUserId);
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