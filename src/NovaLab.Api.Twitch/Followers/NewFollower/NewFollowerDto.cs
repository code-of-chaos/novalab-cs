﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.Followers.NewFollower;

using NovaLab.Data.Data.Twitch.Followers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record NewFollowerDto(
    Guid NewFollowerId,
    Guid FollowerGoalId,
    DateTime TimeStamp,
    string FollowerTwitchUserId
) {
    public static NewFollowerDto FromDbObject(TwitchNewFollower twitchNewFollower) {
        return new NewFollowerDto(
            NewFollowerId: twitchNewFollower.Id, 
            FollowerGoalId: twitchNewFollower.Goal.Id,
            TimeStamp: twitchNewFollower.TimeStamp,
            FollowerTwitchUserId : twitchNewFollower.FollowerTwitchUserId
        );
    }

}