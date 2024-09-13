// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using NovaLab.Server.Database.Models.Streams;
using NovaLab.Server.Database.Models.Streams.HelixApi;

namespace NovaLab.Server.API.Models.Streams;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public record StreamSubjectDto(
    Guid Id,
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
    public static StreamSubjectDto FromDatabase(StreamSubject model, GameTitleToTwitchId? gameCache) {
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