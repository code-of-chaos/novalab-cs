// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using NovaLab.Services.Twitch.EventCallbacks;
using NovaLab.Services.Twitch.EventRegistering;
using NovaLab.Services.Twitch.TwitchTokens;

namespace NovaLab.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchServiceCollection(IServiceCollection serviceCollection) : AbstractServiceCollection(serviceCollection) {
    public override void DefineServices() {
        _serviceCollection.AddHostedService<HostedTwitchWebsocket>();
        _serviceCollection.AddScoped<CatchTwitchManagedReward>();
        _serviceCollection.AddScoped<RegisterCustomRewardRedemption>();
        _serviceCollection.AddScoped<TwitchTokensManager>();
    }
}