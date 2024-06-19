// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser.Attributes;
using CliArgsParser.Contracts;

namespace NovaLab.API.Tools.Args;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DbBackupManualArgs : IParameters {
    [AutoArgValue("file", "Filename to be used. Cannot include .bak")]
    public string FilePath { get; set; } = "temp";
    
    [AutoArgValue("database", "Specific database which you want to have exported")]
    public string DatabaseName { get; set; } = "NovaLabDb";
}
