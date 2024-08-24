// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Database.Contracts;

namespace NovaLab.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchNewFollower : ISoftDeletable {
    [Key]
    public Ulid Id { get; init; }
    public virtual TwitchFollowerGoal Goal { get; init; } = null!; // virtual is being used here by EFC
      
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    [MaxLength(128)] public required string FollowerTwitchUserId { get; set; }
    
    
    #region SoftDelete
    public bool IsSoftDeleted { get; set; } 
    public void SoftDelete() {
        IsSoftDeleted = true;
    }
    #endregion
}
