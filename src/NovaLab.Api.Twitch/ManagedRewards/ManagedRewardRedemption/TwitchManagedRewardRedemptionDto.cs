// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Data.Models.Twitch.Redemptions;

namespace NovaLab.Api.Twitch.ManagedRewards.ManagedRewardRedemption;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record TwitchManagedRewardRedemptionDto(
    Guid ManagedRewardRedemptionId,
    Guid ManagedRewardId,
    DateTime TimeStamp,
    string UserName,
    string? Message
) {
    public static TwitchManagedRewardRedemptionDto FromDbObject(TwitchManagedRewardRedemption redemption) {
        return new TwitchManagedRewardRedemptionDto(
        ManagedRewardRedemptionId:redemption.Id,
        ManagedRewardId:redemption.TwitchManagedReward.Id,
        TimeStamp: redemption.TimeStamp,
        UserName:redemption.Username,
        Message:redemption.Message
        );
    }    
}
