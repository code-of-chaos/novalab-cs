// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Database.Models.Streams;
using NovaLab.Server.Database.Models.Streams.HelixApi;

namespace NovaLab.Server.Database;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabDbContext : IdentityDbContext<NovaLabUser, IdentityRole<Guid>, Guid> {
    #region StreamSubjects
    public DbSet<StreamSubject> StreamSubjects { get; init; }
    public IQueryable<StreamSubject> ActiveStreamSubjects => StreamSubjects.Where(subject => !subject.IsSoftDeleted);
    #endregion
    #region TrackedRewards
    public DbSet<TrackedReward> TrackedRewards { get; init; }
    public IQueryable<TrackedReward> ActiveTrackedRewards => TrackedRewards.Where(goal => !goal.IsSoftDeleted);
    #endregion
    #region TrackedRewardRedemptions
    public DbSet<TrackedRewardRedemption> TrackedRewardRedemptions { get; init; }
    public IQueryable<TrackedRewardRedemption> ActiveTrackedRewardRedemptions => TrackedRewardRedemptions.Where(goal => !goal.IsSoftDeleted);
    #endregion
    #region GameTitleToTwitchIds
    public DbSet<GameTitleToTwitchId> GameTitleToTwitchIds { get; init; }
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
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
    }
}










