// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab;

using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Components;
using Components.Account;
using Data;
using Hosted;
using Hosted.Twitch;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RazorLib.Lib;
using Serilog;
using Serilog.Core;
using Services.Twitch.Hubs;
using Services.Twitch.TwitchTokens;
using System.Text.Json;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets.Extensions;
using static TwitchLib.Api.Core.Common.Helpers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class Program {
    public static void Main(string[] args) {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // -------------------------------------------------------------------------------------------------------------
        // Services
        // -------------------------------------------------------------------------------------------------------------
        // - Logger : SeriLog -
        builder.Logging.ClearProviders();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy( new LoggingLevelSwitch())
            .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
            .WriteTo.Async(lsc => lsc.Console())
            .CreateLogger();
        builder.Logging.AddSerilog(Log.Logger);
        builder.Services.AddSingleton(Log.Logger); // ELse Injecting from Serilog.ILogger won't work
        
        // - Razor components -
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // - Auth  -
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
                //      This might look weird, but the idea is te that we don't reuse this at all anywhere, just create the list and move on
                ((AuthScopes[])[AuthScopes.Helix_Analytics_Read_Extensions, AuthScopes.Helix_Analytics_Read_Games,AuthScopes.Helix_Bits_Read, AuthScopes.Helix_Channel_Edit_Commercial,AuthScopes.Helix_Channel_Manage_Broadcast, AuthScopes.Helix_Channel_Manage_Extensions,AuthScopes.Helix_Channel_Manage_Moderators, AuthScopes.Helix_Channel_Manage_Polls,AuthScopes.Helix_Channel_Manage_Predictions, AuthScopes.Helix_Channel_Manage_Redemptions,AuthScopes.Helix_Channel_Manage_Schedule, AuthScopes.Helix_Channel_Manage_VIPs,AuthScopes.Helix_Channel_Manage_Videos, AuthScopes.Helix_Channel_Read_Charity,AuthScopes.Helix_Channel_Read_Editors, AuthScopes.Helix_Channel_Read_Goals,AuthScopes.Helix_Channel_Read_Hype_Train, AuthScopes.Helix_Channel_Read_Polls,AuthScopes.Helix_Channel_Read_Predictions, AuthScopes.Helix_Channel_Read_Redemptions,AuthScopes.Helix_Channel_Read_Stream_Key, AuthScopes.Helix_Channel_Read_Subscriptions,AuthScopes.Helix_Channel_Read_VIPs, AuthScopes.Helix_Clips_Edit, AuthScopes.Helix_Moderation_Read,AuthScopes.Helix_Moderator_Manage_Announcements, AuthScopes.Helix_Moderator_Manage_Automod,AuthScopes.Helix_Moderator_Manage_Automod_Settings, AuthScopes.Helix_Moderator_Manage_Banned_Users,AuthScopes.Helix_Moderator_Manage_Blocked_Terms, AuthScopes.Helix_Moderator_Manage_Chat_Settings,AuthScopes.Helix_Moderator_Read_Automod_Settings, AuthScopes.Helix_Moderator_Read_Blocked_Terms,AuthScopes.Helix_Moderator_Read_Chat_Settings, AuthScopes.Helix_Moderator_Read_Chatters,AuthScopes.Helix_User_Edit, AuthScopes.Helix_User_Edit_Broadcast,AuthScopes.Helix_User_Edit_Follows, AuthScopes.Helix_User_Manage_BlockedUsers,AuthScopes.Helix_User_Manage_Chat_Color, AuthScopes.Helix_User_Manage_Whispers,AuthScopes.Helix_User_Read_BlockedUsers, AuthScopes.Helix_User_Read_Broadcast,AuthScopes.Helix_User_Read_Email, AuthScopes.Helix_User_Read_Follows,AuthScopes.Helix_User_Read_Subscriptions, AuthScopes.Helix_moderator_Manage_Chat_Messages])
                    .Select(AuthScopesToString)
                    .ToList()
                    .ForEach(twitchOptions.Scope.Add);
                
                // Tokens are stored through ExternalLogin.razor
                //      This is needed to make that work
                twitchOptions.SaveTokens = true;
            })
            .AddBearerToken()
            .AddIdentityCookies();
        
        // - DB -
        string connectionString = builder.Configuration["Database:MariaDb:ConnectionString"]!;
        builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            // options.EnableDetailedErrors();
            // options.EnableSensitiveDataLogging();
            // options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); 
            // options.
        });
        
        builder.Services.AddScoped(options => 
            options.GetRequiredService<IDbContextFactory<NovaLabDbContext>>().CreateDbContext());
        
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<NovaLabUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<NovaLabDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
        
        builder.Services.AddSingleton<IEmailSender<NovaLabUser>, IdentityNoOpEmailSender>();
        
        // - Extra Services - 
        builder.Services.AddScoped<UserAccessor>();
        
        // TwitchApi is a singleton because they don't use injection
        //      Check into if Twitch has an Openapi.json / swagger.json and build own lib with injection?
        builder.Services.AddSingleton(new TwitchAPI {
            Settings = {
                ClientId = builder.Configuration["Authentication:Twitch:ClientId"],
                Secret = builder.Configuration["Authentication:Twitch:ClientSecret"]
            }
        });
        builder.Services.AddTwitchLibEventSubWebsockets(); // Needed by TwitchLib's websockets. I don't remember why.
        
        builder.Services.AddHostedTwitchServices();

        builder.Services.AddScoped<TwitchTokensManager>();
        builder.Services.AddScoped<TwitchHubClient>();

        builder.Services.AddAuthorization();
        builder.Services.AddHttpClient();
        builder.Services.AddHttpClient("TwitchServicesClient", c => { 
            string[]? urls = builder.Configuration["ASPNETCORE_URLS"]?.Split(';', StringSplitOptions.RemoveEmptyEntries);
            string? applicationUrl = urls!.FirstOrDefault();
            c.BaseAddress = new Uri(applicationUrl!); // This fixes some issues, it is janky, but it works
        });
        
        builder.Services.AddControllers().AddJsonOptions(options => {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            // other options...
        });
        
        // - Swagger -
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo {
                Version = "v1",
                Title = "NovaLab API",
                Description = "An ASP.NET Core Web API for managing your streams",
            });
            options.EnableAnnotations();
        });

        builder.Services.AddSignalR();
        // builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
        
        // - Blazorise -
        builder.Services
            .AddBlazorise( options => {
                options.Immediate = true;
            } )
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
        
        // -------------------------------------------------------------------------------------------------------------
        // App
        // -------------------------------------------------------------------------------------------------------------
        WebApplication app = builder.Build();
        if (app.Environment.IsDevelopment()) {
            app.UseMigrationsEndPoint();
            app.UseSwagger();
            app.UseSwaggerUI(options => { // UseSwaggerUI is called only in Development.
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "swagger";
            });
        } else {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization(); 
        app.UseAntiforgery();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        app.MapAdditionalIdentityEndpoints();
        
        // - Hubs -
        app.MapHub<TwitchHub>("/hubs/twitch"); // Add additional endpoints required by the Identity /Account Razor components.
        
        // - Final Run -
        app.Run();
    }
}