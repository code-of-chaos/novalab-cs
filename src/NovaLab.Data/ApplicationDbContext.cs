using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Data.Data.Twitch.Redemptions;

namespace NovaLab.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options) {
    
    public DbSet<TwitchManagedReward> TwitchManagedRewards { get; set; }
    public DbSet<TwitchManagedRewardRedemption> TwitchManagedRewardRedemptions { get; set; }
}