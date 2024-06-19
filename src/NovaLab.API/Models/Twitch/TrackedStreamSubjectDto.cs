// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using NovaLab.Server.Data.Models.Twitch;

namespace NovaLab.API.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public record TrackedStreamSubjectDto(
    Guid Id,
    Guid NovaLabUserId,
    string? TwitchGameId,
    string TwitchBroadcastLanguage,
    string TwitchTitle,
    string[]? TwitchTags,

    Guid? TrackedStreamSubjectComponentId
) {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static TrackedStreamSubjectDto FromDto(TrackedStreamSubject model) {
        return new TrackedStreamSubjectDto(
            Id: model.Id,
            NovaLabUserId: model.User.Id,
            TwitchGameId: model.TwitchGameId,
            TwitchBroadcastLanguage: model.TwitchBroadcastLanguage,
            TwitchTitle: model.TwitchTitle,
            TwitchTags: model.TwitchTags,
            TrackedStreamSubjectComponentId: model.TrackedStreamSubjectComponent?.Id
        );
    }
}