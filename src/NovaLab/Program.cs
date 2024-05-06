// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaLab.Components;
using NovaLab.Components.Account;
using NovaLab.Data;
using TwitchLib.Api;

namespace NovaLab;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class Program {
    public static void Main(string[] args) {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // --- Services ---
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
                twitchOptions.Scope.Add("channel:read:redemptions");
                twitchOptions.Scope.Add("channel:read:subscriptions"); 
                
                // twitchOptions.ClaimActions.MapJsonSubKey("access_token", "data", "access_token");
                                   
                twitchOptions.SaveTokens = true;
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
        app.UseAntiforgery();
        
        // app.UseAuthentication();
        // app.UseAuthorization(); 
        
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