// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Data.Models.Twitch;
using NovaLab.Server.Data.Models.Twitch.HelixApi;

namespace NovaLab.Server.Data;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabDbContext : IdentityDbContext<NovaLabUser, IdentityRole<Guid>, Guid> {
    public DbSet<TrackedStreamSubject> TrackedStreamSubjects { get; init; }
    public IQueryable<TrackedStreamSubject> ActiveTrackedStreamSubjects => TrackedStreamSubjects.Where(subject => !subject.IsSoftDeleted);
    
    public DbSet<TrackedStreamSubjectComponent> TrackedStreamSubjectComponents { get; init; }
    public IQueryable<TrackedStreamSubjectComponent> ActiveTrackedStreamSubjectComponents => TrackedStreamSubjectComponents.Where(subject => !subject.IsSoftDeleted);
    
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