// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
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
            var result = await httpClient.GetFromJsonAsync<ApiResult<T>>(_baseAddress + endpoint, cts.Token);
            return result?.Data ?? [];
        }
        catch(OperationCanceledException) when (cts.IsCancellationRequested) {
            logger.Warning("Operation was cancelled due to timeout for endpoint {endpoint}", endpoint);
            return [];
        }
    }

    protected async Task<T[]> PostToApi<T,TValue>(string endpoint, TValue content, double cancelDelaySeconds = 5)
        where T : notnull 
        where TValue : notnull {

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(cancelDelaySeconds));

        try {
            // var jsonContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(_baseAddress + endpoint, content, cts.Token);
            if (!response.IsSuccessStatusCode) {
                logger.Warning("Failed to POST to endpoint {endpoint}. Status code: {statusCode}", endpoint, response.StatusCode);
                return [];
            }
            
            var result = await response.Content.ReadFromJsonAsync<ApiResult<T>>(cancellationToken: cts.Token);
            return result?.Data ?? [];
        }

        catch (JsonException ex)
        {
            logger.Error(ex, "Failed to serialize/deserialize the response content for endpoint {endpoint}", endpoint);
            return [];
        }
        
        catch (OperationCanceledException) when (cts.IsCancellationRequested) {
            logger.Warning("Operation was cancelled due to timeout for endpoint {endpoint}", endpoint);
            return [];
        }
    }
}