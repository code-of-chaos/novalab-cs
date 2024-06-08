﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Server.Data.Shared.Models.Twitch;

// -----------------------------------------------------------------------------------------------------------------
// Methods
// -----------------------------------------------------------------------------------------------------------------
public record TrackedStreamSubjectDtoPost(
    Guid NovaLabUserId,
    string TwitchTitle,
    string? TwitchBroadcastLanguage = null,
    string[]? TwitchTags = null,
    string? TwitchGameTitleName = null
);
