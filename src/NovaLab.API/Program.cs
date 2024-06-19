// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NovaLab.API.Services.Twitch;
using NovaLab.EnvironmentSwitcher;
using NovaLab.Lib.Twitch;
using NovaLab.Server.Data;
using NovaLab.Server.Data.Models.Account;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using TwitchLib.Api;

namespace NovaLab.API;
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
                options.Variables.TryRegister<string>("TwitchClientId");
                options.Variables.TryRegister<string>("TwitchClientSecret");
            }
        );

        // -------------------------------------------------------------------------------------------------------------
        // Services
        // -------------------------------------------------------------------------------------------------------------
        // - Endpoints -
        builder.Services.AddControllers();
        
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
        
        // - Db -
        try {
            builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
                options.UseSqlServer(environmentSwitcher.DatabaseConnectionString);
            });
            builder.Services.AddScoped(options => 
                options.GetRequiredService<IDbContextFactory<NovaLabDbContext>>().CreateDbContext());
            builder.Services.AddIdentity<NovaLabUser, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<NovaLabDbContext>();
        }
        catch (Exception ex) {
            // ignored
            #if !DEBUG
            if (environmentSwitcher.IsRunningInDocker) throw;
            #else
            // when not in debug, we need to create the swagger.json
            //      THis makes it so that the swagger.json is generated correctly without having access to the database
            Log.Logger.Warning(ex, "Database connection could not be established");
            #endif
        }
        
        // - Kestrel SLL - 
        builder.WebHost.ConfigureKestrel(options => {
            options.ConfigureHttpsDefaults(opt => {
                opt.ServerCertificate = new X509Certificate2( 
                environmentSwitcher.SslCertLocation, 
                environmentSwitcher.SslCertPassword);
            });
        });
        
        // - Cors -
        builder.Services.AddCors(options => {
            options.AddPolicy("AllowLocalHosts", policyBuilder => { policyBuilder
                .WithOrigins(
                    // Local Development 
                    "https://localhost:7145","http://localhost:5117", 
                    // Docker 
                    "http://localhost:9052", "https://localhost:9052", // API
                    "http://localhost:9051", "https://localhost:9051",  // Server
                    "https://localhost:80"
                )
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
            });
        });
        
        // - Twitch Services -
        // TwitchApi is a singleton because they don't use injection
        //      Check into if Twitch has an Openapi.json / swagger.json and build own lib with injection?
        try {
            builder.Services.AddSingleton(new TwitchAPI {
                Settings = {
                    ClientId = environmentSwitcher.TwitchClientId,
                    Secret = environmentSwitcher.TwitchClientSecret
                }
            });
            builder.Services.AddScoped<TwitchTokensManager>();
            builder.Services.AddSingleton<TwitchGameTitleToIdCacheService>();
        } 
        catch (Exception ex) {
            // ignored
            #if !DEBUG
            if (environmentSwitcher.IsRunningInDocker) throw;
            #else
            Log.Logger.Warning(ex, "Twitch could not be added to the API");
            #endif
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // App
        // -------------------------------------------------------------------------------------------------------------
        WebApplication app = builder.Build();
        
        // - Configure the HTTP request pipeline -
        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        // - Cors -
        app.UseCors("AllowLocalHosts");
        
        // - Swagger -
        app.UseSwagger();
        app.UseSwaggerUI(ctx => {
            ctx.SwaggerEndpoint("/swagger/v1/swagger.json", "NovaLab API");
            ctx.RoutePrefix = string.Empty;
        });

        // - Custom endpoints -
        app.MapGet("", ctx => {
            ctx.Response.Redirect("/swagger/index.html");
            return Task.CompletedTask;
        });
        
        app.MapControllers();
        await app.RunAsync().ConfigureAwait(false);
    }
}
