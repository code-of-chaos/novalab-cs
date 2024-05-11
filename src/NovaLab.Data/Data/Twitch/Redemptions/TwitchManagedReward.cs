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
    
    public virtual required ApplicationUser User { get; set; } // virtual is being used here by EFC
    [MaxLength(128)] public required string RewardId { get; set; }
    
    [MaxLength(45)]  public required string Title { get; set;}
    public required int PointsCost { get; set;}
    public required bool HasPrompt { get; set; }

    public string OutputTemplatePerReward { get; set; } = "{msg}";
    public string OutputTemplate { get; set; } = "Custom NovaLab Reward : \n {rewardTemplate}";

    public DateTime LastCleared { get; set; } = DateTime.MinValue;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
}
