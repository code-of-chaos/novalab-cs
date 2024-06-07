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
    [AutoArgValue("path")]public string FilePath { get; set; } = "temp";
    [AutoArgValue("database")]public string DatabaseName { get; set; } = "CatalDocV2Db";
}
