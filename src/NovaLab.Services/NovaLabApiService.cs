// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Configuration;
using NovaLab.ApiClient.Api;
using NovaLab.ApiClient.Client;

namespace NovaLab.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabApiService(IConfiguration configuration) {
    private readonly Configuration _configuration = new() { BasePath = configuration["ApiEndpoint"]! };
    
    private TrackedStreamSubjectApi? _trackedStreamSubjectApi;
    public TrackedStreamSubjectApi TrackedStreamSubject => _trackedStreamSubjectApi ??= new TrackedStreamSubjectApi(_configuration);
}
