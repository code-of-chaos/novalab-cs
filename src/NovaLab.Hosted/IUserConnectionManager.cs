// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace NovaLab.Hosted;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUserConnectionManager {
    void StoreUserConnection(string userId, string? connectionId);
    void RemoveUserConnection(string userId);
    string? GetConnectionId(string userId);
    bool TryGetConnectionId(string userId, [NotNullWhen(true)] out string? connectionId);
}
