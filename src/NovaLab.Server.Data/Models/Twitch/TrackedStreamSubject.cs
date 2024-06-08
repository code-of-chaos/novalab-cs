// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using ISOLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NovaLab.Server.Data.Models.Account;
using NovaLab.Server.Data.Shared;
using NovaLab.Server.Data.Shared.Models.Twitch;
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
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public TrackedStreamSubjectDto ToDto() {
        return new TrackedStreamSubjectDto(
            Id:Id,
            NovaLabUserId: User.Id,
            TwitchGameId:TwitchGameId,
            TwitchBroadcastLanguage:TwitchBroadcastLanguage,
            TwitchTitle:TwitchTitle,
            TwitchTags:TwitchTags,
            TrackedStreamSubjectComponentId:TrackedStreamSubjectComponent?.Id
        );
    }
}