// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Server.Database.Contracts;

namespace NovaLab.Server.Database.Models.Streams;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TrackedRewardRedemption : Content {
    public virtual required TrackedReward TrackedReward { get; set; }
    
    public DateTime TimeStamp { get; set; }
    [MaxLength(128)] public required string Username { get; set; }
    [MaxLength(255)] public string? Message { get; set; }
}