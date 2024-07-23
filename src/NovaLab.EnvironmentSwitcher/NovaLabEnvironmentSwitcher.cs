// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.AspNetCore.Environment;
using CodeOfChaos.Extensions.Serilog;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace NovaLab.EnvironmentSwitcher;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NovaLabEnvironmentSwitcher : CodeOfChaos.AspNetCore.Environment.EnvironmentSwitcher {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public bool IsRunningInDocker => Variables.GetRequiredValue<bool>(Enum.GetName(PremadeEnvironmentVariablesBools.RunningInDocker)!);
    public string SslCertLocation => Variables.GetRequiredValue<string>(Enum.GetName(PreMadeEnvironmentVariablesStrings.SslCertLocation)!);
    public string SslCertPassword => Variables.GetRequiredValue<string>(Enum.GetName(PreMadeEnvironmentVariablesStrings.SslCertPassword)!);
    
    public string DockerDb => Variables.GetRequiredValue<string>(nameof(DockerDb));
    public string DevelopmentDb => Variables.GetRequiredValue<string>(nameof(DockerDb));
    
    public string TwitchClientId => Variables.GetRequiredValue<string>(nameof(TwitchClientId));
    public string TwitchClientSecret => Variables.GetRequiredValue<string>(nameof(TwitchClientSecret));
    
    /// <summary>
    /// Retrieves the database connection string based on the running environment.
    /// </summary>
    /// <returns>
    /// The database connection string.
    /// </returns>
    /// <exception cref="ApplicationException">Thrown when no connection string could be determined.</exception>
    [UsedImplicitly]
    public string DatabaseConnectionString { get  {
        if (IsRunningInDocker) {
            // Program delivering "builder" is running in a docker container
            return DockerDb;
        }

        // Program delivering "builder" is NOT running in a docker container
        //      AKA: probably in some local dev's test environment
        if (Variables.TryGetValue(nameof(DevelopmentDb), out string? value)) return value;
        if (Configuration.GetConnectionString("DefaultConnection") is {} defaultConnectionString) return defaultConnectionString;

        // All possible routes exhausted
        Log.Logger.ThrowFatal<ApplicationException>("No Connection string could be determined");
        return string.Empty;// TODO check why ThrowFatal doesn't NOT RETURN for the IDE
        }
    }
    
}
