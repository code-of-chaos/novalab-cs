// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Database.Contracts;

namespace NovaLab.Database.Models.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TrackedStreamSubjectComponent : ISoftDeletable{
    [Key]
    public Guid Id { get; init; }
    
    public Guid TrackedStreamSubjectId { get; init; }
    public TrackedStreamSubject TrackedStreamSubject { get; init; } = null!;

    [MaxLength(255)] public string ComponentText { get; set; } = string.Empty;
    [MaxLength(255)] public string ComponentStyling { get; set; } = string.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    #region SoftDelete
    public bool IsSoftDeleted { get; set; } 
    public void SoftDelete() {
        IsSoftDeleted = true;
    }
    #endregion
}













