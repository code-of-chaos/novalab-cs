// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Configuration;
using NovaLab.ApiClient;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using NovaLab.ApiClient.Api.Twitch.TrackedStreamSubject;

namespace NovaLab.WasmClient.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabApiService(IConfiguration configuration) {
    public readonly NovaLabApiClient NovaLabApiClient = new(new HttpClientRequestAdapter(
        new AnonymousAuthenticationProvider(),
        httpClient: new HttpClient { BaseAddress = new Uri(configuration["ApiEndpoint"]!) }
    ));
    
    
    private  TrackedStreamSubjectRequestBuilder ? _trackedStreamSubjectApi;
    public  TrackedStreamSubjectRequestBuilder  TrackedStreamSubject => _trackedStreamSubjectApi ??= NovaLabApiClient.Api.Twitch.TrackedStreamSubject;
}
