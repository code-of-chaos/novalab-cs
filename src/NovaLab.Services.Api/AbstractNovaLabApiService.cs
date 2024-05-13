// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using NovaLab.Api;
using Serilog;

namespace NovaLab.Services.Api;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class AbstractNovaLabApiService(HttpClient httpClient, NavigationManager navigationManager, ILogger logger) {
    private readonly Uri _baseAddress = new(navigationManager.BaseUri);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected async Task<T[]> GetFromApi<T>(string endpoint, double cancelDelaySeconds = 5) {
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(cancelDelaySeconds));

        try {
            var result = await httpClient.GetFromJsonAsync<ApiResultDto<T>>(_baseAddress + endpoint, cts.Token);
            return result?.Data ?? [];
        }
        catch(OperationCanceledException) when (cts.IsCancellationRequested) {
            logger.Warning("Operation was cancelled due to timeout for endpoint {endpoint}", endpoint);
            return [];
        }
    }

    protected async Task<T[]> PostToApi<T,TValue>(string endpoint, TValue content, double cancelDelaySeconds = 5)
        where T : class 
        where TValue : class {

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(cancelDelaySeconds));

        try {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(_baseAddress + endpoint, content, cts.Token);
            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<T>>(cancellationToken: cts.Token);
            return result?.Data ?? [];
        }

        catch (OperationCanceledException) when (cts.IsCancellationRequested) {
            logger.Warning("Operation was cancelled due to timeout for endpoint {endpoint}", endpoint);
            return [];
        }
    }
}