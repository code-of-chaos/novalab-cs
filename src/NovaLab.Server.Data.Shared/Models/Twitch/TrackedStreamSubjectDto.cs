// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Server.Data.Shared.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record TrackedStreamSubjectDto(
    Guid Id,
    Guid NovaLabUserId,
    string? TwitchGameId,
    string TwitchBroadcastLanguage,
    string TwitchTitle,
    string[]? TwitchTags,
    
    Guid? TrackedStreamSubjectComponentId
);