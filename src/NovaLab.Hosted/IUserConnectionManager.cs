// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace NovaLab.Hosted;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUserConnectionManager {
    bool TryStoreUserConnection(string userId, string connectionId);
    bool TryRemoveUserConnection(string userId);
    bool TryGetConnectionId(string userId, [NotNullWhen(true)] out string? connectionId);
}
