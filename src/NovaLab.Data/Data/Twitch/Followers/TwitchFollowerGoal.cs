// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Data.Data.Twitch.Followers;

using System.ComponentModel.DataAnnotations;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchFollowerGoal {
    [Key]
    public Guid Id { get; init; }
    public virtual NovaLabUser User { get; init; } = null!; // virtual is being used here by EFC
    
    public int DailyGoal { get; set; } = 1;
    [MaxLength(5)] public string Divider = "/";
    [MaxLength(255)] public string? CustomCssStyling { get; set; }
}
