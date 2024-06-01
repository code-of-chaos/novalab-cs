// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace NovaLab.Data.Data.Twitch.Streams;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TwitchManagedStreamSubject {
    [Key]
    public Guid Id { get; init; }
    public virtual NovaLabUser User { get; init; } = null!; // virtual is being used here by EFC
    
    [MaxLength(32)]
    public string SelectionName { get; set; } = "Undefined";
    
    [MaxLength(128)]
    public string ObsSubjectTitle { get; set; } = "UNDEFINED";
    
    [MaxLength(140)]
    public string TwitchSubjectTitle { get; set; } = "UNDEFINED";
    
}
