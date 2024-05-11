// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using NovaLab.Data.Data.Twitch.Redemptions;
using Serilog;

namespace NovaLab.Api;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class ApiService(HttpClient httpClient, NavigationManager navigationManager, ILogger logger) {
    private readonly Uri _baseAddress = new(navigationManager.BaseUri);

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private async Task<T[]> GetFromApi<T>(string endpoint, double cancelDelaySeconds = 5) where T : class {
        logger.Warning(_baseAddress + endpoint);
        
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(cancelDelaySeconds));
        
        try {
            HttpResponseMessage response = await httpClient.GetAsync(_baseAddress + endpoint, cts.Token);
            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<T?>>(cancellationToken: cts.Token);
            return (result?.Data ?? [])!;
        }
        catch(OperationCanceledException) when (cts.IsCancellationRequested) {
            logger.Warning("Operation was cancelled due to timeout for endpoint {endpoint}", endpoint);
            return [];
        }
    }
    
    private async Task<T[]> PostToApi<T>(string endpoint, HttpContent? content, double cancelDelaySeconds = 5) where T : class {
        logger.Warning(_baseAddress + endpoint);
        
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(cancelDelaySeconds));

        try {
            HttpResponseMessage response = await httpClient.PostAsync(_baseAddress + endpoint, content, cts.Token);
            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<T?>>(cancellationToken: cts.Token);
            return (result?.Data ?? [])!;
        }
        catch (TwitchLib.Api.Core.Exceptions.BadScopeException e) {
            logger.Warning(e, "Scope was blocked due to bad credentials");
            // TODO refresh token?
            return [];
        }

        catch (OperationCanceledException) when (cts.IsCancellationRequested) {
            logger.Warning("Operation was cancelled due to timeout for endpoint {endpoint}", endpoint);
            return [];
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Endpoint Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task<TwitchManagedReward[]> GetCustomTwitchRedemptions(string userId) {
        return await GetFromApi<TwitchManagedReward>($"api/{userId}/twitch/redemptions/git-commit-message");
    }
    
    public async Task<TwitchManagedReward[]> PostCustomTwitchRedemptions(string userId) {
        return await PostToApi<TwitchManagedReward>($"api/{userId}/twitch/redemptions/git-commit-message",null);
    }
}