// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;

namespace NovaLab.Server.Tools.Args;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DbBackupManualArgs : ICommandParameters {
    [ArgValue("file", "Filename to be used. Cannot include .bak")]
    public string FilePath { get; set; } = "temp";
    
    [ArgValue("database", "Specific database which you want to have exported")]
    public string DatabaseName { get; set; } = "NovaLabDb";
}

