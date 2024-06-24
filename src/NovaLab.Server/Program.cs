// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Blazorise;
using Blazorise.Icons.FontAwesome;
using Blazorise.Tailwind;
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.AspNetCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaLab.Client.Lib.Services;
using NovaLab.EnvironmentSwitcher;
using NovaLab.Lib.Twitch;
using NovaLab.Server.Components;
using NovaLab.Server.Components.Account;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Models.Account;
using System.Security.Cryptography.X509Certificates;
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

        var environmentSwitcher = builder.CreateEnvironmentSwitcher<NovaLabEnvironmentSwitcher>(
            options => {
                options.DefinePreMadeVariables();
                options.Variables.TryRegister<string>("DevelopmentDb");
                options.Variables.TryRegister<string>("ApiUrlRoot");
                options.Variables.TryRegister<string>("TwitchClientId");
                options.Variables.TryRegister<string>("TwitchClientSecret");
            }
        );
        
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
                twitchOptions.ClientId = environmentSwitcher.TwitchClientId;
                twitchOptions.ClientSecret = environmentSwitcher.TwitchClientSecret;
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
        builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
            options.UseSqlServer(environmentSwitcher.DatabaseConnectionString);
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
                opt.ServerCertificate = new X509Certificate2( 
                environmentSwitcher.SslCertLocation, 
                environmentSwitcher.SslCertPassword);
            });
        });
        
        // - Twitch Services -
        // TwitchApi is a singleton because they don't use injection
        //      Check into if Twitch has an Openapi.json / swagger.json and build own lib with injection?
        builder.Services.AddSingleton(new TwitchAPI {
            Settings = {
                ClientId = environmentSwitcher.TwitchClientId,
                Secret = environmentSwitcher.TwitchClientSecret
            }
        });
        builder.Services.AddScoped<TwitchTokensManager>();
        // builder.Services.AddTwitchLibEventSubWebsockets(); // Needed by TwitchLib's websockets. I don't remember why.
        // builder.Services.AddHostedTwitchServices();

        // - Blazorise -
        builder.Services
            .AddBlazorise( options => {
                options.Immediate = true;
            })
            .AddTailwindProviders()
            .AddTailwindComponents()
            .AddFontAwesomeIcons();
        
        // - Cors -
        builder.Services.AddCors(options => {
            options.AddPolicy("AllowLocalHosts", policyBuilder => {
                policyBuilder
                    .WithOrigins(
                    // Local Development 
                    "https://localhost:7190", "https://localhost:7145", 
                    // Docker 
                    "http://localhost:9052", "https://localhost:9052", // API
                    "http://localhost:9051", "https://localhost:9051", // Server
                    "https://localhost:80"
                )
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
            });
        });

        
        builder.Services.AddScoped<UserService>();
        
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
            .AddAdditionalAssemblies(typeof(Client.Program).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();
        app.MapGet("/api", httpContext => {
            httpContext.Response.Redirect(environmentSwitcher.Variables.GetRequiredValue<string>("ApiUrlRoot"));
            return Task.CompletedTask;
        });
        

        await app.RunAsync().ConfigureAwait(false);
    }
}