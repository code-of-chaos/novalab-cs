// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity;
using NovaLab.Data.Models.Twitch.Followers;
using NovaLab.Data.Models.Twitch.Streams;

namespace NovaLab.Data;

using System.ComponentModel.DataAnnotations;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabUser : IdentityUser {
    [MaxLength(16)]
    public string? TwitchBroadcasterId { get; set; }
    
    // Navigation property for the one-to-one relationship
    public virtual TwitchFollowerGoal? TwitchFollowerGoal { get; set; }
    
    
    public Guid? SelectedManagedStreamSubjectId { get; set; }
    public virtual TwitchManagedStreamSubject? SelectedManagedStreamSubject { get; set; }
}