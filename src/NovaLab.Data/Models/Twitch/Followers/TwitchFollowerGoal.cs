// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace NovaLab.Data.Models.Twitch.Followers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchFollowerGoal {
    [Key]
    public Guid Id { get; init; }
    
    // Foreign key property
    [MaxLength(255)] public string UserId { get; set; } = null!;
    // Navigation property for the one-to-one relationship
    public virtual NovaLabUser User { get; init; } = null!;
    
    public int DailyGoal { get; set; } = 1;
    [MaxLength(5)] public string Divider = "/";
    [MaxLength(255)] public string? CustomCssStyling { get; set; }
}
