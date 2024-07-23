// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;
using CliArgsParser.Contracts;
using CliArgsParser.PreMade.Commands;
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NovaLab.EnvironmentSwitcher;
using NovaLab.Server.Data;

namespace NovaLab.API.Tools;
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

        // -------------------------------------------------------------------------------------------------------------
        // App
        // -------------------------------------------------------------------------------------------------------------
        WebApplication app = builder.Build();

        List<Type> atlasTypes = [
            typeof(HelpCommand), // TODO pester the dev of CliArgsParser into making an actually decent Help Command
            typeof(DbCommandAtlas),
        ];
        
        // Configure parser & load all Command atlases
        var parserConfiguration = new ParserConfiguration();
        atlasTypes.ForEach(t => parserConfiguration.RegisterAtlas(ActivatorUtilities.CreateInstance(app.Services, t)));

        // Create parser and treat the system like a CLI
        ICliParser parser = parserConfiguration.CreateCliParser();
        await parser.TryParseContinuousAsync();
    }
}
