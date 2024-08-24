// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Database.Contracts;

namespace NovaLab.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchFollowerGoal : ISoftDeletable {
    [Key]
    public Ulid Id { get; init; }
    public virtual NovaLabUser User { get; init; } = null!;
    
    public int DailyGoal { get; set; } = 1;
    [MaxLength(5)] public string Divider = "/";
    [MaxLength(255)] public string? CustomCssStyling { get; set; }
    
    #region SoftDelete
    public bool IsSoftDeleted { get; set; } 
    public void SoftDelete() {
        IsSoftDeleted = true;
    }
    #endregion
    
}
