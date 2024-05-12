// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NovaLab.Services.Twitch.EventRegistering;

namespace NovaLab.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[UsedImplicitly]
public class TwitchRegisterAtWebsocket(IServiceScope scope) : AbstractScopedProcessor(scope){
    public RegisterCustomRewardRedemption RegisterCustomRewardRedemption => GetRequiredService<RegisterCustomRewardRedemption>();
}