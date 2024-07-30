// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Server.Data.Contracts;

namespace NovaLab.Server.Data.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TrackedStreamSubjectComponent : ISoftDeletable{
    [Key]
    public Guid Id { get; init; }
    public bool IsSoftDeleted { get; set; } 
    
    public Guid TrackedStreamSubjectId { get; init; }
    public TrackedStreamSubject TrackedStreamSubject { get; init; } = null!;

    [MaxLength(255)] public string ComponentText { get; set; } = string.Empty;
    [MaxLength(255)] public string ComponentStyling { get; set; } = string.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SoftDelete() {
        IsSoftDeleted = true;
    }
}













