// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NovaLab.Server.Data.Models.Account;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public class NovaLabUser : IdentityUser<Guid> {
    [MaxLength(32)] public string? TwitchBroadcasterId { get; set; } = string.Empty;
}
