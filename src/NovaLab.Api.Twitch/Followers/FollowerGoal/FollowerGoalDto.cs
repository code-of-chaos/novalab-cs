// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.Followers.FollowerGoal;

using NovaLab.Data.Data.Twitch.Followers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record FollowerGoalDto(
    string UserId,
    string? TwitchBroadcasterId,
    Guid GoalId,
    int DailyGoalAmount,
    string Divider,
    string CustomCssStyling
) {

    public static FollowerGoalDto FromDbObject(TwitchFollowerGoal twitchFollowerGoal) {
        return new FollowerGoalDto(
            UserId: twitchFollowerGoal.User.Id,
            TwitchBroadcasterId: twitchFollowerGoal.User.TwitchBroadcasterId,
            GoalId: twitchFollowerGoal.Id,
            DailyGoalAmount: twitchFollowerGoal.DailyGoal,
            Divider: twitchFollowerGoal.Divider,
            CustomCssStyling: twitchFollowerGoal.CustomCssStyling ?? ""
        );
    }
    
}
