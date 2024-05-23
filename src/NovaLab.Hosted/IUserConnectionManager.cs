// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace NovaLab.Hosted;

using System.Collections.Concurrent;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUserConnectionManager {
    bool TryStoreUserConnection(string userId, string connectionId);
    bool TryRemoveUserConnection(string userId);
    bool TryGetConnectionId(string userId, [NotNullWhen(true)] out string? connectionId);
    
    ConcurrentDictionary<string, string> Map { get; }
}
