// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace NovaLab.Server.Data.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TrackedStreamSubjectComponent {
    [Key]
    public Guid Id { get; init; }
    
    public Guid TrackedStreamSubjectId { get; init; }
    public TrackedStreamSubject TrackedStreamSubject { get; init; } = null!;

    [MaxLength(255)] public string ComponentText { get; set; } = string.Empty;
    [MaxLength(255)] public string ComponentStyling { get; set; } = string.Empty;
}













