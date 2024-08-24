// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using NovaLab.Database.Contracts;

namespace NovaLab.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchManagedRewardRedemption : ISoftDeletable {
    [Key]
    public Ulid Id { get; set; }
    public virtual required TwitchManagedReward TwitchManagedReward { get; set; }
    
    public DateTime TimeStamp { get; set; }
    [MaxLength(128)] public required string Username { get; set; }
    [MaxLength(255)] public string? Message { get; set; }
    
    
    #region SoftDelete
    public bool IsSoftDeleted { get; set; } 
    public void SoftDelete() {
        IsSoftDeleted = true;
    }
    #endregion
}