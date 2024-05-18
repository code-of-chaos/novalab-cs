// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace NovaLab.Data.Data.Twitch.Redemptions;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchManagedReward {
    [Key]
    public Guid Id { get; set; }
    public virtual ApplicationUser User { get; set; } = null!; // virtual is being used here by EFC
    
    // Delivered by Twitch
    [MaxLength(128)] public string RewardId { get; set; } = null!;
    
    [Required(ErrorMessage = "Output Template Per Reward is required")]
    [MaxLength(255)]
    public string OutputTemplatePerReward { get; set; } = "{msg}";
    
    [Required(ErrorMessage = "Output Template is required")]
    [MaxLength(255)]
    public string OutputTemplate { get; set; } = "Custom NovaLab Reward : \n {rewardTemplate}";

    public DateTime LastCleared { get; set; } = DateTime.MinValue;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
}
