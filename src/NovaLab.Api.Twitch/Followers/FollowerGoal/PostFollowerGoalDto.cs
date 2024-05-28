﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Api.Twitch.Followers.FollowerGoal;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record PostFollowerGoalDto(
    string UserId,
    int DailyGoal = 1,
    string Divider = "/",
    string? CustomCssStyling = null
);