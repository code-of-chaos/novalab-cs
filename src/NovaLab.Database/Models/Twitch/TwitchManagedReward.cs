// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Database.Contracts;

namespace NovaLab.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchManagedReward : ISoftDeletable{
    [Key]
    public Ulid Id { get; init; }
    public virtual NovaLabUser User { get; init; } = null!; // virtual is being used here by EFC
    
    // Delivered by Twitch
    [MaxLength(128)] public string RewardId { get; init; } = null!;
    
    [Required(ErrorMessage = "Output Template Per Reward is required")]
    [MaxLength(255)]
    public string OutputTemplatePerRedemption { get; set; } = "{msg}";
    
    [Required(ErrorMessage = "Output Template is required")]
    [MaxLength(255)]
    public string OutputTemplatePerReward { get; set; } = "Custom NovaLab Reward : \n {rewardTemplate}";

    public DateTime LastCleared { get; set; } = DateTime.MinValue;

    public bool IsValidOnTwitch { get; set; } = true;

    
    #region SoftDelete
    public bool IsSoftDeleted { get; set; } 
    public void SoftDelete() {
        IsSoftDeleted = true;
    }
    #endregion
    
    // -----------------------------------------------------------------------------------------------------------------
    // Navigation Properties
    // -----------------------------------------------------------------------------------------------------------------
    // Navigation Property
    public virtual ICollection<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; set; } = [];
}
