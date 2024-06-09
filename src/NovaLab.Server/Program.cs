// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.AspNetCore;
using dotenv.net;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Components;
using NovaLab.Server.Components.Account;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Models.Account;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using static TwitchLib.Api.Core.Common.Helpers;

namespace NovaLab.Server;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    public async static Task Main(string[] args) {
        // -------------------------------------------------------------------------------------------------------------
        // Builder Setup
        // -------------------------------------------------------------------------------------------------------------
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.OverrideLoggingAsSeriLog();
        builder.Configuration.AddEnvironmentVariables(); // Else they won't be loaded

        var environmentSwitcher = new EnvironmentSwitcher(Log.Logger, builder);
        
        // -------------------------------------------------------------------------------------------------------------
        // Services
        // -------------------------------------------------------------------------------------------------------------
        // - Pages & Components-
        builder.Services.AddRazorPages();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // - Identity & Auth -
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
        
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(options => {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddTwitch(twitchOptions => {
                twitchOptions.ClientId = environmentSwitcher.GetTwitchClientId();
                twitchOptions.ClientSecret = environmentSwitcher.GetTwitchClientSecret();
                // Update scopes as needed
                //      This might look weird, but the idea is te that we don't reuse this at all anywhere, just create the list and move on
                ((AuthScopes[]) [
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
                        AuthScopes.Helix_Channel_Manage_Videos,
                        AuthScopes.Helix_Channel_Manage_VIPs,
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
                        AuthScopes.Helix_Moderator_Manage_Banned_Users,
                        AuthScopes.Helix_Moderator_Manage_Blocked_Terms,
                        AuthScopes.Helix_Moderator_Manage_Announcements,
                        AuthScopes.Helix_Moderator_Manage_Automod,
                        AuthScopes.Helix_Moderator_Manage_Automod_Settings,
                        AuthScopes.Helix_moderator_Manage_Chat_Messages,
                        AuthScopes.Helix_Moderator_Manage_Chat_Settings,
                        AuthScopes.Helix_Moderator_Read_Blocked_Terms,
                        AuthScopes.Helix_Moderator_Read_Automod_Settings,
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
                        AuthScopes.Helix_Moderator_Read_Followers,
                    ])
                    .Select(AuthScopesToString)
                    .ToList()
                    .ForEach(twitchOptions.Scope.Add);

                // Tokens are stored through ExternalLogin.razor
                //      This is needed to make that work
                twitchOptions.SaveTokens = true;
            })
            .AddCookie()
            .AddBearerToken()
        ;
        
        // - Db -
        string connectionString = environmentSwitcher.GetDatabaseConnectionString();
        builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
            options.UseSqlServer(connectionString);
        });
        builder.Services.AddScoped(options => 
            options.GetRequiredService<IDbContextFactory<NovaLabDbContext>>().CreateDbContext());
        
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentity<NovaLabUser, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<NovaLabDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            ;

        builder.Services.AddSingleton<IEmailSender<NovaLabUser>, IdentityNoOpEmailSender>();
        
        // - Kestrel SLL - 
        builder.WebHost.ConfigureKestrel(options => {
            options.ConfigureHttpsDefaults(opt => {
                opt.ServerCertificate = new X509Certificate2( environmentSwitcher.GetSslCertLocation(), environmentSwitcher.GetSslCertPassword());
            });
        });
        
        // - Twitch Services -
        // TwitchApi is a singleton because they don't use injection
        //      Check into if Twitch has an Openapi.json / swagger.json and build own lib with injection?
        builder.Services.AddSingleton(new TwitchAPI {
            Settings = {
                ClientId = environmentSwitcher.GetTwitchClientId(),
                Secret = environmentSwitcher.GetTwitchClientSecret()
            }
        });
        // builder.Services.AddTwitchLibEventSubWebsockets(); // Needed by TwitchLib's websockets. I don't remember why.
        // builder.Services.AddHostedTwitchServices();

        builder.Services.AddScoped<TwitchTokensManager>();
        
        // - Blazorise -
        builder.Services
            .AddBlazorise( options => {
                options.Immediate = true;
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
        
        // - Cors -
        builder.Services.AddCors(options => {
            options.AddPolicy("AllowLocalHosts", policyBuilder => { policyBuilder
                .WithOrigins(
                    // Local Development 
                    "https://localhost:7190", 
                    // Docker 
                    "http://localhost:9052", "https://localhost:9052"
                )
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
            });
        });

        // -------------------------------------------------------------------------------------------------------------
        // NovaLabApp
        // -------------------------------------------------------------------------------------------------------------
        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
        }
        else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        
        
        // - Cors -
        app.UseCors("AllowLocalHosts");

        app.UseAuthentication();
        app.UseAuthorization(); 
        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<NovaLabApp>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        await app.RunAsync().ConfigureAwait(false);
    }
}
