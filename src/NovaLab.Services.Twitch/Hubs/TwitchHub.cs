// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.SignalR;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Hosted;
using Serilog;

namespace NovaLab.Services.Twitch.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TwitchHub(IUserConnectionManager userConnectionManager, ILogger logger, IHttpContextAccessor httpContextAccessor) : Hub {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public override async Task OnConnectedAsync() {
        await base.OnConnectedAsync();

        if (httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value is {} user) {
            logger.Warning(user);
        };
        
        if (Context.User is null) {
            logger.Warning("User could not be defined");
            return;
        }
        string? id = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null) {
            logger.Warning("Id could not be defined");
            return;
        }
        
        string userId = Guid.Parse(id).ToString();
        if (!userConnectionManager.TryStoreUserConnection(userId, Context.ConnectionId)) {
            logger.Warning("Identity {identity} could not be added to connection manager", userId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        string userId = Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!).ToString();
        if (!userConnectionManager.TryRemoveUserConnection(userId)) {
            logger.Warning("Identity {identity} could not be removed from connection manager", userId);
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