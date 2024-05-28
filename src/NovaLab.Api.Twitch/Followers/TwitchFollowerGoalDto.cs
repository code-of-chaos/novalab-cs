// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.Followers;

using Data.Data.Twitch.Followers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record TwitchFollowerGoalDto(
    string UserId,
    Guid GoalId,
    int DailyGoalAmount,
    string Divider,
    string CustomCssStyling
) {

    public static TwitchFollowerGoalDto FromDbObject(TwitchFollowerGoal twitchFollowerGoal) {
        return new TwitchFollowerGoalDto(
            UserId: twitchFollowerGoal.User.Id,
            GoalId: twitchFollowerGoal.Id,
            DailyGoalAmount: twitchFollowerGoal.DailyGoal,
            Divider: twitchFollowerGoal.Divider,
            CustomCssStyling: twitchFollowerGoal.CustomCssStyling ?? ""
        );
    }
    
}
