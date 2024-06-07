// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Data.Models.Account;

namespace NovaLab.Server.Data;

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabDbContext : IdentityDbContext<NovaLabUser, IdentityRole<Guid>, Guid> {
    
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