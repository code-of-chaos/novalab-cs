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
    public void StoreUserConnection(string userId, string connectionId) {
        _userMap.TryAdd(userId, connectionId);
    }

    public void RemoveUserConnection(string userId) {
        _userMap.TryRemove(userId, out _);
    }

    public string? GetConnectionId(string userId) {
        return _userMap.GetValueOrDefault(userId);
    }

    public bool TryGetConnectionId(string userId, [NotNullWhen(true)] out string? connectionId) {
        return _userMap.TryGetValue(userId, out connectionId);
    }
}