// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Abstractions;
using NovaLab.ApiClient;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using NovaLab.ApiClient.Api.Twitch.TrackedStreamSubject;

namespace NovaLab.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabApiService(IConfiguration configuration, HttpClient client, IAuthenticationProvider authenticationProvider) {
    public readonly NovaLabApiClient NovaLabApiClient = new(new HttpClientRequestAdapter(
        authenticationProvider,
        httpClient: new HttpClient { BaseAddress = new Uri(configuration["ApiEndpoint"]!) }
    ));
    
    
    private  TrackedStreamSubjectRequestBuilder ? _trackedStreamSubjectApi;
    public  TrackedStreamSubjectRequestBuilder  TrackedStreamSubject => _trackedStreamSubjectApi ??= NovaLabApiClient.Api.Twitch.TrackedStreamSubject;
}
