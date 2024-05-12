// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace NovaLab.Services.Api;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class NovaLapApiServiceCollection(IServiceCollection serviceCollection) : AbstractServiceCollection(serviceCollection){
    public override void DefineServices() {
        _serviceCollection.AddScoped<NovaLabApiService>();
    }
}