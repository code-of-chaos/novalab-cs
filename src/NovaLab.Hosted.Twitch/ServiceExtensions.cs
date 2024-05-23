// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Hosted.Twitch;

using EventCallbacks;
using EventRegistering;
using Microsoft.Extensions.DependencyInjection;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public static class ServiceExtensions {
    public static IServiceCollection AddHostedTwitchServices(this IServiceCollection serviceCollection) {
        serviceCollection.AddHostedService<TwitchEventSubWebsocket>();
        
        serviceCollection.AddScoped<CatchTwitchManagedReward>();
        serviceCollection.AddScoped<RegisterCustomRewardRedemption>();
        
        return serviceCollection;
    } 
} 
