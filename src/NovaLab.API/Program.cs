// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Data;
using Serilog;
using System.Security.Cryptography.X509Certificates;

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
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
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
                opt.ServerCertificate = new X509Certificate2( environmentSwitcher.GetSslCertLocation(), environmentSwitcher.GetSslCertPassword());
            });
        });
        
        // -------------------------------------------------------------------------------------------------------------
        // App
        // -------------------------------------------------------------------------------------------------------------
        WebApplication app = builder.Build();
        
        // - Configure the HTTP request pipeline -
        app.UseHttpsRedirection();
        app.UseAuthorization();
        
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
