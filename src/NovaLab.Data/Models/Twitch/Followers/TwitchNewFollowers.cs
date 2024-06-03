// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace NovaLab.Data.Models.Twitch.Followers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchNewFollower {
    [Key]
    public Guid Id { get; init; }
    public virtual TwitchFollowerGoal Goal { get; init; } = null!; // virtual is being used here by EFC
      
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    [MaxLength(128)] public required string FollowerTwitchUserId { get; set; }
}
