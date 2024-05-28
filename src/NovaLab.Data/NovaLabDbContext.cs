// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data.Data.Twitch.Redemptions;

namespace NovaLab.Data;

using Data.Twitch.Followers;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabDbContext : IdentityDbContext<NovaLabUser> {
    public DbSet<TwitchManagedReward> TwitchManagedRewards { get; set; }
    public DbSet<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; set; }
    
    public DbSet<TwitchFollowerGoal> TwitchFollowerGoals { get; set; }
    public DbSet<TwitchNewFollower> TwitchNewFollowers { get; set; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public NovaLabDbContext() {}
    public NovaLabDbContext(DbContextOptions<NovaLabDbContext> options) : base(options) {}
}