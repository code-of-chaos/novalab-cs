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
public class NovaLabDbContext : IdentityDbContext<NovaLabUser> {
    public DbSet<TwitchManagedReward> TwitchManagedRewards { get; set; }
    public DbSet<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; set; }

    public NovaLabDbContext() {}
    public NovaLabDbContext(DbContextOptions<NovaLabDbContext> options) : base(options) {}
}