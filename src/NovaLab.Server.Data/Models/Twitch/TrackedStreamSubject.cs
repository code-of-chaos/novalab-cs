// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using ISOLib;
using NovaLab.Server.Data.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace NovaLab.Server.Data.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TrackedStreamSubject {
    [Key]
    public Guid Id { get; init; }
    public virtual NovaLabUser User { get; init; } = null!;
    
    [MaxLength(140)] public string? TwitchGameId { get; set; }
    [MaxLength(2)] public string TwitchBroadcastLanguage { get; set; } = Languages.EN.Alpha2;
    [MaxLength(140)] public string TwitchTitle { get; set; } = "NOVALAB : Undefined Stream Title";
    public string[]? TwitchTags { get; set; } = [];
    
    public TrackedStreamSubjectComponent? TrackedStreamSubjectComponent { get; set; }
}