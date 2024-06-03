// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.ManagedRewards.ManagedReward;

using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record PostManagedRewardDto(
    string UserId,
    CreateCustomRewardsRequest TwitchApiRequest,
    string OutputTemplatePerReward,
    string OutputTemplatePerRedemption
);
