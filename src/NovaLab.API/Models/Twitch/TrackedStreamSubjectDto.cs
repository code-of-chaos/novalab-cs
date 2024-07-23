// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using NovaLab.Server.Data.Models.Twitch;
using NovaLab.Server.Data.Models.Twitch.HelixApi;

namespace NovaLab.API.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public record TrackedStreamSubjectDto(
    Guid Id,
    Guid NovaLabUserId,
    string TwitchGameId,
    string TwitchGameName,
    string TwitchGameImageUrl,
    string TwitchBroadcastLanguage,
    string TwitchTitle,
    string[] TwitchTags,

    Guid? TrackedStreamSubjectComponentId
) {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static TrackedStreamSubjectDto FromDto(TrackedStreamSubject model, TwitchGameTitleToIdCache? gameCache) {
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