// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Server.Database.Models.Streams.HelixApi;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GameTitleToTwitchId {
    [Key]
    [MaxLength(255)] public string NovaLabName { get; set; } = null!;
    [MaxLength(255)] public string TwitchTitleId { get; set; } = null!;
    [MaxLength(255)] public string TwitchTitleName { get; set; } = null!;
    [MaxLength(255)] public string TwitchTitleBoxArtUrl { get; set; } = null!;
    [MaxLength(255)] public string? TwitchTitleIgdbId { get; set; }
}