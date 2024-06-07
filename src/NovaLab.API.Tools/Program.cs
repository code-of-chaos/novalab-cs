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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NovaLab.Server.Data;
using Serilog;

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
        builder.Configuration.AddEnvironmentVariables(); // Else they won't be loaded

        var environmentSwitcher = new EnvironmentSwitcher(Log.Logger, builder);

        // -------------------------------------------------------------------------------------------------------------
        // Services
        // -------------------------------------------------------------------------------------------------------------
        // - Db -
        string connectionString = environmentSwitcher.GetDatabaseConnectionString();
        builder.Services.AddDbContextFactory<NovaLabDbContext>(options => {
            options.UseSqlServer(connectionString);
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
