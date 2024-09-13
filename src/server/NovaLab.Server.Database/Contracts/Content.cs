// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Server.Database.Contracts;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class Content : ISoftDeletable {
    [Key] public Guid Id { get; set; }  // TODO DotNet9 => Change to UUIDv7 
    
    #region SoftDelete
    public bool IsSoftDeleted { get; private set; } 
    public void SoftDelete() => IsSoftDeleted = true;
    #endregion
}
