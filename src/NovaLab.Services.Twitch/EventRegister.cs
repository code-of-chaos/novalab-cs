// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using DependencyInjectionMadeEasy;
using NovaLab.Services.Twitch.EventRegistering;

namespace NovaLab.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[DiScoped]
public record EventRegister(
    ChannelPointsCustomRewardRedemptionAdd ChannelPointsCustomRewardRedemptionAdd    
) ;