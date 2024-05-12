// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using NovaLab.Services.Twitch.EventCallbacks;

namespace NovaLab.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchEventsCallback(IServiceScope scope) : AbstractScopedProcessor(scope){
    public CatchTwitchManagedReward CatchTwitchManagedReward => GetRequiredService<CatchTwitchManagedReward>();
}