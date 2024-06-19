// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace NovaLab.Server.Data.Models.Twitch.HelixApi;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TwitchGameTitleToIdCache {
    [Key]
    [MaxLength(255)] public string NovaLabName { get; set; } = null!;
    [MaxLength(255)] public string TwitchTitleId { get; set; } = null!;
    [MaxLength(255)] public string TwitchTitleName { get; set; } = null!;
    [MaxLength(255)] public string TwitchTitleBoxArtUrl { get; set; } = null!;
    [MaxLength(255)] public string? TwitchTitleIgdbId { get; set; }
}