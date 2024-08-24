// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using ISOLib;
using NovaLab.Database.Contracts;
using NovaLab.Database.Models.Account;

namespace NovaLab.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TrackedStreamSubject : ISoftDeletable{
    [Key]
    public Ulid Id { get; init; }
    public virtual NovaLabUser User { get; init; } = null!;
    
    [MaxLength(140)] public string? TwitchGameId { get; set; }
    [MaxLength(2)] public string TwitchBroadcastLanguage { get; set; } = Languages.EN.Alpha2;
    [MaxLength(140)] public string TwitchTitle { get; set; } = "NOVALAB : Undefined Stream Title";
    public string[]? TwitchTags { get; set; } = [];
    
    public TrackedStreamSubjectComponent? TrackedStreamSubjectComponent { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    #region SoftDelete
    public bool IsSoftDeleted { get; set; } 
    public void SoftDelete() {
        IsSoftDeleted = true;
    }
    #endregion
}