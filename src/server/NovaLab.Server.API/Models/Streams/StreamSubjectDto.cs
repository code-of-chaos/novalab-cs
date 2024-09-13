// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using NovaLab.Server.Database.Models.Twitch;
using NovaLab.Server.Database.Models.Twitch.HelixApi;

namespace NovaLab.Server.API.Models.Streams;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public record StreamSubjectDto(
    Ulid Id,
    Guid NovaLabUserId,
    string TwitchGameId,
    string TwitchGameName,
    string TwitchGameImageUrl,
    string TwitchBroadcastLanguage,
    string TwitchTitle,
    string[] TwitchTags
) {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static StreamSubjectDto FromDatabase(TwitchStreamSubject model, TwitchGameTitleToIdCache? gameCache) {
        return new StreamSubjectDto(
            Id: model.Id,
            NovaLabUserId: model.User.Id,
            TwitchGameId: gameCache?.TwitchTitleId ?? "",
            TwitchGameName : gameCache?.TwitchTitleName ?? "",
            TwitchGameImageUrl : gameCache?.TwitchTitleBoxArtUrl ?? "",
            TwitchBroadcastLanguage: model.TwitchBroadcastLanguage,
            TwitchTitle: model.TwitchTitle,
            TwitchTags: model.TwitchTags ?? []
        );
    }
}