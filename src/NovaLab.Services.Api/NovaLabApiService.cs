// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using NovaLab.Api;
using NovaLab.Data.Data.Twitch.Redemptions;
using Serilog;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;

namespace NovaLab.Services.Api;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabApiService(HttpClient httpClient, NavigationManager navigationManager, ILogger logger) : AbstractNovaLabApiService(httpClient, navigationManager, logger) {

    // -----------------------------------------------------------------------------------------------------------------
    // Endpoint Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task<TwitchManagedReward[]> GetCustomTwitchRedemptions(string userId) {
        return await GetFromApi<TwitchManagedReward>($"api/{userId}/twitch/redemptions/git-commit-message");
    }
    
    public async Task<TwitchManagedReward[]> PostCustomTwitchRedemptions(string userId, CreateCustomRewardsRequest createCustomRewardsRequest) {
        return await PostToApi<TwitchManagedReward, CreateCustomRewardsRequest>($"api/{userId}/twitch/redemptions/git-commit-message",createCustomRewardsRequest);
    }

    public async Task<bool> RefreshTwitchAccessToken(string userId) {
        bool[] result = await GetFromApi<bool>($"api/{userId}/twitch/tokens/refresh");
        return result.All(r => r);
    }
}