// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.SignalR;
using NovaLab.Data.Data.Twitch.Redemptions;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace NovaLab.Services.Twitch.Hub;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchHub : Microsoft.AspNetCore.SignalR.Hub {
    public async Task SendNewManagedRewardRedemption(string userId, TwitchManagedRewardRedemption newRecord) {
        await Clients.User(userId).SendAsync(TwitchHubMethods.NewManagedRewardRedemption, newRecord);
    }
}