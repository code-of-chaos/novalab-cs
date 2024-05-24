// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.ManagedRewards;

using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record PostManagedRewardDto(
    CreateCustomRewardsRequest TwitchApiRequest,
    string OutputTemplatePerReward,
    string OutputTemplatePerRedemption
    
);
