// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Hosted.Twitch;

using Events.CustomRewardRedemption;
using Events.TwitchFollow;
using Microsoft.Extensions.DependencyInjection;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public static class ServiceExtensions {
    public static IServiceCollection AddHostedTwitchServices(this IServiceCollection serviceCollection) {
        serviceCollection.AddHostedService<TwitchEventSubWebsocket>();
        
        serviceCollection.AddScoped<CatchTwitchManagedReward>();
        serviceCollection.AddScoped<CatchTwitchFollow>();
        serviceCollection.AddScoped<RegisterCustomRewardRedemption>();
        serviceCollection.AddScoped<RegisterTwitchFollow>();
        
        return serviceCollection;
    } 
} 
