// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NovaLab.Server.Data;
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
        builder.Configuration.AddEnvironmentVariables(); // Else they won't be loaded

        var environmentSwitcher = new EnvironmentSwitcher(Log.Logger, builder);

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
        string connectionString = environmentSwitcher.GetDatabaseConnectionString();
        builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
            options.UseSqlServer(connectionString);
        });
        builder.Services.AddScoped(options => 
            options.GetRequiredService<IDbContextFactory<NovaLabDbContext>>().CreateDbContext());
        
        // - Kestrel SLL - 
        builder.WebHost.ConfigureKestrel(options => {
            options.ConfigureHttpsDefaults(opt => {
                opt.ServerCertificate = new X509Certificate2( 
                environmentSwitcher.GetSslCertLocation(), 
                environmentSwitcher.GetSslCertPassword());
            });
        });
        
        // - Cors -
        builder.Services.AddCors(options => {
            options.AddPolicy("AllowLocalHosts", policyBuilder => { policyBuilder
                .WithOrigins(
                    // Local Development 
                    "https://localhost:7145;http://localhost:5117", 
                    // Docker 
                    "http://localhost:9052", "https://localhost:9052"
                )
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
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
