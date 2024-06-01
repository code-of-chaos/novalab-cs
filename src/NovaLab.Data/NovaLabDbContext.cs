// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data.Data.Twitch.Redemptions;
using NovaLab.Data.Data.Twitch.Streams;

namespace NovaLab.Data;

using Data.Twitch.Followers;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabDbContext : IdentityDbContext<NovaLabUser> {
    public DbSet<TwitchManagedReward> TwitchManagedRewards { get; init; }
    public DbSet<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; init; }
    
    public DbSet<TwitchFollowerGoal> TwitchFollowerGoals { get; init; }
    public DbSet<TwitchNewFollower> TwitchNewFollowers { get; init; }
    
    public DbSet<TwitchManagedStreamSubject> TwitchManagedStreamSubjects { get; init; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public NovaLabDbContext() {}
    public NovaLabDbContext(DbContextOptions<NovaLabDbContext> options) : base(options) {}

    // -----------------------------------------------------------------------------------------------------------------
    // Model Creating
    // -----------------------------------------------------------------------------------------------------------------
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NovaLabUser>()
            .HasIndex(u => u.TwitchBroadcasterId)
            .IsUnique();
        
        modelBuilder.Entity<NovaLabUser>()
            .HasOne(r => r.TwitchFollowerGoal)
            .WithOne(fg => fg.User)
            .HasForeignKey<TwitchFollowerGoal>(fg => fg.UserId);
    }
}