// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace NovaLab.Data.Data.Twitch.Redemptions;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class TwitchManagedRewardRedemption {
    [Key]
    public Guid Id { get; set; }
    public virtual required TwitchManagedReward TwitchManagedReward { get; set; }
    
    public DateTime TimeStamp { get; set; }
    [MaxLength(128)] public required string Username { get; set; }
    [MaxLength(255)] public string? Message { get; set; }
}

public record TwitchManagedRewardRedemptionDto(
    string RewardId,
    string Username,
    string? Message
);