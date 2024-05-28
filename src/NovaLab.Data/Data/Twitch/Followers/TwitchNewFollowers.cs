// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Data.Data.Twitch.Followers;

using System.ComponentModel.DataAnnotations;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchNewFollower {
    [Key]
    public Guid Id { get; init; }
    public virtual TwitchFollowerGoal Goal { get; init; } = null!; // virtual is being used here by EFC
      
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    [MaxLength(128)] public string? FollowerTwitchUserId { get; set; }
}
