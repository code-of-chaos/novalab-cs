// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data.Data.Twitch.Redemptions;

namespace NovaLab.Data;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabDbContext(DbContextOptions<NovaLabDbContext> options)
    : IdentityDbContext<NovaLabUser>(options) {
    
    public DbSet<TwitchManagedReward> TwitchManagedRewards { get; set; }
    public DbSet<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; set; }
}