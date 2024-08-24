// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using ISOLib;
using NovaLab.Database.Contracts;

namespace NovaLab.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchStreamSubject : UserContent {
    [MaxLength(140)] public string? TwitchGameId { get; set; }
    [MaxLength(2)] public string TwitchBroadcastLanguage { get; set; } = Languages.EN.Alpha2;
    [MaxLength(140)] public string TwitchTitle { get; set; } = "NOVALAB : Undefined Stream Title";
    public string[]? TwitchTags { get; set; } = [];
}