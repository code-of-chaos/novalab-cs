using Microsoft.AspNetCore.Identity;

namespace NovaLab.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public string? TwitchBroadcasterId { get; set; }
    public string? TwitchAccessToken { get; set; }
}