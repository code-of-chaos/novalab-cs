// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using NovaLab.Database.Models.Twitch;
using NovaLab.Database.Models.Twitch.HelixApi;

namespace NovaLab.API.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public record TrackedStreamSubjectDto(
    Ulid Id,
    Guid NovaLabUserId,
    string TwitchGameId,
    string TwitchGameName,
    string TwitchGameImageUrl,
    string TwitchBroadcastLanguage,
    string TwitchTitle,
    string[] TwitchTags,

    Ulid? TrackedStreamSubjectComponentId
) {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static TrackedStreamSubjectDto FromDatabase(TrackedStreamSubject model, TwitchGameTitleToIdCache? gameCache) {
        return new TrackedStreamSubjectDto(
            Id: model.Id,
            NovaLabUserId: model.User.Id,
            TwitchGameId: gameCache?.TwitchTitleId ?? "",
            TwitchGameName : gameCache?.TwitchTitleName ?? "",
            TwitchGameImageUrl : gameCache?.TwitchTitleBoxArtUrl ?? "",
            TwitchBroadcastLanguage: model.TwitchBroadcastLanguage,
            TwitchTitle: model.TwitchTitle,
            TwitchTags: model.TwitchTags ?? [],
            TrackedStreamSubjectComponentId: model.TrackedStreamSubjectComponent?.Id
        );
    }
}