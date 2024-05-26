// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.SignalR;
namespace NovaLab.Services.Twitch.Hubs;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TwitchHub : Hub {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task AddToGroup(string userId) {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await Clients.Group($"user_{userId}").SendAsync("Send", $"{Context.ConnectionId} has joined the group");
    }
    
    public async Task RemoveFromGroup(string userId) {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        await Clients.Group($"user_{userId}").SendAsync("Send", $"{Context.ConnectionId} has left the group.");
    }
}