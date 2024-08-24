// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Database.Contracts;

namespace NovaLab.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchManagedRewardRedemption : Content {
    public virtual required TwitchManagedReward TwitchManagedReward { get; set; }
    
    public DateTime TimeStamp { get; set; }
    [MaxLength(128)] public required string Username { get; set; }
    [MaxLength(255)] public string? Message { get; set; }
}