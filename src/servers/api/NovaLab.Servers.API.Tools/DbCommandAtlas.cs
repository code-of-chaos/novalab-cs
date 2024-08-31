// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using NovaLab.Server.Database;
using NovaLab.Servers.API.Tools.Args;
using System.Diagnostics.CodeAnalysis;
using ILogger=Serilog.ILogger;

namespace NovaLab.Servers.API.Tools;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DbCommandAtlas(IDbContextFactory<NovaLabDbContext> contextFactory, ILogger logger) : ICommandAtlas {
    private Task<NovaLabDbContext> NovaLabDb => contextFactory.CreateDbContextAsync();

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private bool TrySanitizeFilePath(string input, [NotNullWhen(true)] out string? output) {
        output = null;
        if (input.Contains(' ')) return false;
        if (input.Contains('\'')) return false;

        try {
            output = Path.ChangeExtension(input, "");
            return true;
        }
        catch (Exception ex) {
            logger.Error(ex, "Failed to parse output path.");
            return false;
        }
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Command Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Command<DbBackupManualArgs>("db-backup-manual"), UsedImplicitly]
    public async Task CmdDbBackupManual(DbBackupManualArgs args) {
        try {
            await using NovaLabDbContext dbContext = await NovaLabDb;

            logger.Information("Sanitizing FilePath ...");
            if (!TrySanitizeFilePath(args.FilePath, out string? sanitizedFilePath)) {
                logger.Warning("Filepath {path} could not be sanitized", args.FilePath);
                return;
            }

            logger.Information("Creating Backup at Db");
            string backupCommand = $"BACKUP DATABASE {args.DatabaseName} TO DISK='/var/opt/mssql/backups/{sanitizedFilePath + DateTime.UtcNow}.bak'";
            await dbContext.Database.ExecuteSqlRawAsync(backupCommand);

            logger.Information("Backup created at {path}, at db location", $"{sanitizedFilePath}.bak");
        }
        catch (Exception ex) {
            logger.Error(ex, "Unexpected exception Occured");
        }
    }
    
    [Command("db-migrate"),UsedImplicitly]
    public async Task CmdDbMigrate() {
        try {
            await using NovaLabDbContext dbContext = await NovaLabDb;

            logger.Information("Starting migration ...");
            await dbContext.Database.MigrateAsync();

            logger.Information("Migration Ended");
        }
        catch (Exception ex) {
            logger.Error(ex, "Unexpected exception Occured");
        }

    }
}
