// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Extensions.SeriLog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CodeOfChaos.AspNetCore.Environment;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class EnvironmentSwitcher(ILogger logger, WebApplicationBuilder builder) {
    private readonly EnvironmentVariables _variables = new(builder.Configuration);
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public string GetDatabaseConnectionString() {
        if (_variables.RunningInDocker) {
            // Program delivering "builder" is running in a docker container
            return _variables.DockerDb;
        }

        // Program delivering "builder" is NOT running in a docker container
        //      AKA: probably in some local dev's test environment
        if (_variables.TryGetDevelopmentDb(out string? value)) return value;
        if (builder.Configuration.GetConnectionString("DefaultConnection") is {} defaultConnectionString) return defaultConnectionString;
        
        // All possible routes exhausted
        logger.ThrowFatal<ApplicationException>("No Connection string could be determined");
        return string.Empty; // TODO check why ThrowFatal doesn't NOT RETURN for the IDE
    }

    public string GetSslCertLocation() => _variables.SslCertLocation; // currently only set through the same ENV variable whether in Docker or dev
    public string GetSslCertPassword() => _variables.SslCertPassword;

    public string GetTwitchClientId() => _variables.TwitchClientId;
    public string GetTwitchClientSecret() => _variables.TwitchClientSecret;
}
