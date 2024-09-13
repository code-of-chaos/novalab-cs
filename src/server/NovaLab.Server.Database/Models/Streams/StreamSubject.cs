// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using ISOLib;
using NovaLab.Server.Database.Contracts;

namespace NovaLab.Server.Database.Models.Streams;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StreamSubject : UserContent {
    [MaxLength(140)] public string? TwitchGameId { get; set; }
    [MaxLength(2)] public string TwitchBroadcastLanguage { get; set; } = Languages.EN.Alpha2;
    [MaxLength(140)] public string TwitchTitle { get; set; } = "NOVALAB : Undefined Stream Title";
    public string[]? TwitchTags { get; set; } = [];
}