// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Database.Models.Twitch;
using NovaLab.Server.Database.Models.Twitch.HelixApi;

namespace NovaLab.Server.Database;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabDbContext : IdentityDbContext<NovaLabUser, IdentityRole<Guid>, Guid> {
    #region TwitchStreamSubject
    public DbSet<TwitchStreamSubject> TwitchStreamSubject { get; init; }
    public IQueryable<TwitchStreamSubject> ActiveTwitchStreamSubject => TwitchStreamSubject.Where(subject => !subject.IsSoftDeleted);
    #endregion
    #region TwitchManagedRewards
    public DbSet<TwitchManagedReward> TwitchManagedRewards { get; init; }
    public IQueryable<TwitchManagedReward> ActiveTwitchManagedRewards => TwitchManagedRewards.Where(goal => !goal.IsSoftDeleted);
    #endregion
    #region TwitchManagedRewardRedemptions
    public DbSet<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; init; }
    public IQueryable<TwitchManagedRewardRedemption> ActiveTwitchManagedRewardRedemptions => TwitchManagedRewardRedemptions.Where(goal => !goal.IsSoftDeleted);
    #endregion
    #region TwitchGameTitleToIdCache
    public DbSet<TwitchGameTitleToIdCache> TwitchGameTitleToIdCache { get; init; }
    #endregion
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
    }
}










