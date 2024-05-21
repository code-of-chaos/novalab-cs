// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace NovaLab.Hosted;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UserConnectionManager : IUserConnectionManager {
    private readonly ConcurrentDictionary<string, string> _userMap = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public bool TryStoreUserConnection(string userId, string connectionId) => _userMap.TryAdd(userId, connectionId);
    public bool TryRemoveUserConnection(string userId) => _userMap.TryRemove(userId, out _);
    public bool TryGetConnectionId(string userId, [NotNullWhen(true)] out string? connectionId) => _userMap.TryGetValue(userId, out connectionId);
}