// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Server.Database.Contracts;

namespace NovaLab.Server.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchManagedReward : UserContent{
    [MaxLength(128)] public required string TwitchRewardId { get; init; }
    
    [MaxLength(255)] public string TemplatePerRedemption { get; set; } = "- {username} : \"{msg}\"";
    [MaxLength(255)] public string TemplateTotal { get; set; } = "Custom NovaLab Reward : {newLine} {templatePerRedemption}";

    public DateTime LastCleared { get; set; } = DateTime.MinValue;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Navigation Properties
    // -----------------------------------------------------------------------------------------------------------------
    // Navigation Property
    public virtual ICollection<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; set; } = [];
}
