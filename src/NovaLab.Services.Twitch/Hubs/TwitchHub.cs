﻿// ---------------------------------------------------------------------------------------------------------------------
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
    public async Task AddToGroup(string userId) {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await Clients.Group($"user_{userId}").SendAsync("Send", $"{Context.ConnectionId} has joined the group");
    }
    
    public async Task RemoveFromGroup(string userId) {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        await Clients.Group($"user_{userId}").SendAsync("Send", $"{Context.ConnectionId} has left the group.");
    }
}