// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaLab.Api;
using NovaLab.Components;
using NovaLab.Components.Account;
using NovaLab.Data;
using Serilog;
using Serilog.Core;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using static TwitchLib.Api.Core.Common.Helpers;

namespace NovaLab;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class Program {
    public static void Main(string[] args) {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // --- Services ---
        builder.Logging.ClearProviders();
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy( new LoggingLevelSwitch())
            .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
            .WriteTo.Async(lsc => lsc.Console())
            .CreateLogger();
        builder.Logging.AddSerilog(Log.Logger);
        builder.Services.AddSingleton(Log.Logger); // ELse Injecting from Serilog.ILogger won't work
        
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(options => {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddTwitch(twitchOptions => {
                twitchOptions.ClientId = builder.Configuration["Authentication:Twitch:ClientId"]!;
                twitchOptions.ClientSecret = builder.Configuration["Authentication:Twitch:ClientSecret"]!;
                
                // Update scopes as needed
                AuthScopes[] scopes = [
                    AuthScopes.Helix_Analytics_Read_Extensions,
                    AuthScopes.Helix_Analytics_Read_Games,
                    AuthScopes.Helix_Bits_Read,
                    AuthScopes.Helix_Channel_Edit_Commercial,
                    AuthScopes.Helix_Channel_Manage_Broadcast,
                    AuthScopes.Helix_Channel_Manage_Extensions,
                    AuthScopes.Helix_Channel_Manage_Moderators,
                    AuthScopes.Helix_Channel_Manage_Polls,
                    AuthScopes.Helix_Channel_Manage_Predictions,
                    AuthScopes.Helix_Channel_Manage_Redemptions,
                    AuthScopes.Helix_Channel_Manage_Schedule,
                    AuthScopes.Helix_Channel_Manage_VIPs,
                    AuthScopes.Helix_Channel_Manage_Videos,
                    AuthScopes.Helix_Channel_Read_Charity,
                    AuthScopes.Helix_Channel_Read_Editors,
                    AuthScopes.Helix_Channel_Read_Goals,
                    AuthScopes.Helix_Channel_Read_Hype_Train,
                    AuthScopes.Helix_Channel_Read_Polls,
                    AuthScopes.Helix_Channel_Read_Predictions,
                    AuthScopes.Helix_Channel_Read_Redemptions,
                    AuthScopes.Helix_Channel_Read_Stream_Key,
                    AuthScopes.Helix_Channel_Read_Subscriptions,
                    AuthScopes.Helix_Channel_Read_VIPs,
                    AuthScopes.Helix_Clips_Edit,
                    AuthScopes.Helix_Moderation_Read,
                    AuthScopes.Helix_Moderator_Manage_Announcements,
                    AuthScopes.Helix_Moderator_Manage_Automod,
                    AuthScopes.Helix_Moderator_Manage_Automod_Settings,
                    AuthScopes.Helix_Moderator_Manage_Banned_Users,
                    AuthScopes.Helix_Moderator_Manage_Blocked_Terms,
                    AuthScopes.Helix_Moderator_Manage_Chat_Settings,
                    AuthScopes.Helix_Moderator_Read_Automod_Settings,
                    AuthScopes.Helix_Moderator_Read_Blocked_Terms,
                    AuthScopes.Helix_Moderator_Read_Chat_Settings,
                    AuthScopes.Helix_Moderator_Read_Chatters,
                    AuthScopes.Helix_User_Edit,
                    AuthScopes.Helix_User_Edit_Broadcast,
                    AuthScopes.Helix_User_Edit_Follows,
                    AuthScopes.Helix_User_Manage_BlockedUsers,
                    AuthScopes.Helix_User_Manage_Chat_Color,
                    AuthScopes.Helix_User_Manage_Whispers,
                    AuthScopes.Helix_User_Read_BlockedUsers,
                    AuthScopes.Helix_User_Read_Broadcast,
                    AuthScopes.Helix_User_Read_Email,
                    AuthScopes.Helix_User_Read_Follows,
                    AuthScopes.Helix_User_Read_Subscriptions,
                    AuthScopes.Helix_moderator_Manage_Chat_Messages,
                ];
                scopes
                    .Select(AuthScopesToString)
                    .ToList()
                    .ForEach(twitchOptions.Scope.Add);
                
                twitchOptions.SaveTokens = true;
                // Tokens are stored through ExternalLogin.razor
            })
            .AddBearerToken()
            .AddIdentityCookies();

        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                                  throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        builder.Services.AddSingleton<TwitchAPI>();
        builder.Services.AddScoped<TwitchTokenProvider>();

        builder.Services.AddAuthorization();
        builder.Services.AddHttpClient();
        builder.Services.AddTransient<ApiService>();
        
        builder.Services.AddControllers();
        
        // --- APP ---
        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseMigrationsEndPoint();
        } else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        // app.UseAuthentication();
        // app.UseAuthorization();
        
        app.UseHttpsRedirection();

        app.UseStaticFiles();
        
        app.UseAuthentication();
        app.UseAuthorization(); 
        app.UseAntiforgery();
        
        app.MapControllers();
        // app.MapRazorPages();
        
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();
        
        // TwitchApi is a singleton because they don't use injection
        var api = app.Services.GetService<TwitchAPI>()!;
        api.Settings.ClientId = builder.Configuration["Authentication:Twitch:ClientId"];
        api.Settings.Secret = builder.Configuration["Authentication:Twitch:ClientSecret"];

        
        app.Run();
    }
}