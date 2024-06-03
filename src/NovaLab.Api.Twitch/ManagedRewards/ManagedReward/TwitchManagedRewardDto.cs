// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Data.Models.Twitch.Redemptions;

namespace NovaLab.Api.Twitch.ManagedRewards.ManagedReward;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record TwitchManagedRewardDto(
    Guid ManagedRewardId,
    string UserId,
    string TwitchBroadcasterId,
    string TwitchRewardId,
    string OutputTemplatePerRedemption,
    string OutputTemplatePerReward
) {
    public static TwitchManagedRewardDto FromDbObject(TwitchManagedReward managedReward) {
        return new TwitchManagedRewardDto(
            ManagedRewardId:managedReward.Id,
            UserId:managedReward.User.Id,
            TwitchBroadcasterId: managedReward.User.TwitchBroadcasterId!,
            TwitchRewardId:managedReward.RewardId,
            OutputTemplatePerRedemption:managedReward.OutputTemplatePerRedemption,
            OutputTemplatePerReward:managedReward.OutputTemplatePerReward
        );
    }
}
