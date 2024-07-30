// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using ISOLib;
using NovaLab.Server.Data.Contracts;

namespace NovaLab.Server.Data.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TrackedStreamSubject : ISoftDeletable{
    [Key]
    public Guid Id { get; init; }
    public virtual NovaLabUser User { get; init; } = null!;
    public bool IsSoftDeleted { get; set; } 
    
    [MaxLength(140)] public string? TwitchGameId { get; set; }
    [MaxLength(2)] public string TwitchBroadcastLanguage { get; set; } = Languages.EN.Alpha2;
    [MaxLength(140)] public string TwitchTitle { get; set; } = "NOVALAB : Undefined Stream Title";
    public string[]? TwitchTags { get; set; } = [];
    
    public TrackedStreamSubjectComponent? TrackedStreamSubjectComponent { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SoftDelete() {
        IsSoftDeleted = true;
    }
}