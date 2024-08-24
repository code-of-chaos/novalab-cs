// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Database.Models.Twitch;
using NovaLab.Database.Models.Twitch.HelixApi;

namespace NovaLab.Database;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabDbContext : IdentityDbContext<NovaLabUser, IdentityRole<Guid>, Guid> {
    public DbSet<TrackedStreamSubject> TrackedStreamSubjects { get; init; }
    public IQueryable<TrackedStreamSubject> ActiveTrackedStreamSubjects => TrackedStreamSubjects.Where(subject => !subject.IsSoftDeleted);
    
    public DbSet<TrackedStreamSubjectComponent> TrackedStreamSubjectComponents { get; init; }
    public IQueryable<TrackedStreamSubjectComponent> ActiveTrackedStreamSubjectComponents => TrackedStreamSubjectComponents.Where(subject => !subject.IsSoftDeleted);
    
    public DbSet<TwitchFollowerGoal> TwitchFollowerGoals { get; init; }
    public IQueryable<TwitchFollowerGoal> ActiveTwitchFollowerGoals => TwitchFollowerGoals.Where(goal => !goal.IsSoftDeleted);
    
    public DbSet<TwitchManagedReward> TwitchManagedRewards { get; init; }
    public IQueryable<TwitchManagedReward> ActiveTwitchManagedRewards => TwitchManagedRewards.Where(goal => !goal.IsSoftDeleted);
    
    public DbSet<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; init; }
    public IQueryable<TwitchManagedRewardRedemption> ActiveTwitchManagedRewardRedemptions => TwitchManagedRewardRedemptions.Where(goal => !goal.IsSoftDeleted);
    
    public DbSet<TwitchNewFollower> TwitchNewFollowers { get; init; }
    public IQueryable<TwitchNewFollower> ActiveTwitchNewFollowers => TwitchNewFollowers.Where(goal => !goal.IsSoftDeleted);
    
    public DbSet<TwitchGameTitleToIdCache> TwitchGameTitleToIdCache { get; init; }
    
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
        
        modelBuilder.Entity<TrackedStreamSubject>()
            .HasOne(p => p.TrackedStreamSubjectComponent)
            .WithOne(t => t.TrackedStreamSubject)
            .HasForeignKey<TrackedStreamSubjectComponent>(rem => rem.Id)
            .IsRequired(false);

        modelBuilder.Entity<TrackedStreamSubjectComponent>()
            .HasIndex(b => b.Id)
            .IsUnique();
    }
}