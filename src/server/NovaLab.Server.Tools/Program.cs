// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NovaLab.Server.EnvironmentSwitcher;
using NovaLab.Server.Database;

namespace NovaLab.Server.Tools;
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
        // - Db -
        builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
            options.UseSqlServer(environmentSwitcher.DatabaseConnectionString);
        });
        builder.Services.AddScoped(options => 
            options.GetRequiredService<IDbContextFactory<NovaLabDbContext>>().CreateDbContext());
        
        builder.Services.AddCliArgsParser(configuration =>
            configuration
                .SetConfig(new CliArgsParserConfig {
                    Overridable = true,
                    GenerateShortNames = true,
                    EnableExitAtlas = true,
                    EnableHelpAtlas = true
                })
                .AddFromAssembly(typeof(Program).Assembly)
        );

        // -------------------------------------------------------------------------------------------------------------
        // App
        // -------------------------------------------------------------------------------------------------------------
        WebApplication app = builder.Build();
        
        // Configure parser & load all Command atlases
        var parser = app.Services.GetRequiredService<ICliParser>();
        await parser.StartParsingAsync();
    }
}
