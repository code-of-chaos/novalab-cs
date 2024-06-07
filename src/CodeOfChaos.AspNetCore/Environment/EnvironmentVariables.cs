// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.AspNetCore.Environment;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class EnvironmentVariables(IConfigurationManager configuration) {
    public bool RunningInDocker => bool.TryParse(configuration["RunningInDocker"], out bool output) && output;
    
    public string DockerDb => GetRequiredEnvVar(nameof(DockerDb));
    public bool TryGetDockerDb([NotNullWhen(true)] out string? value) => TryGetEnvVar(nameof(DockerDb), out value);
    
    public string DockerApi => GetRequiredEnvVar(nameof(DockerApi));
    public bool TryGetDockerApi([NotNullWhen(true)] out string? value) => TryGetEnvVar(nameof(DockerApi), out value);
    
    public string DevelopmentDb => GetRequiredEnvVar(nameof(DevelopmentDb));
    public bool TryGetDevelopmentDb([NotNullWhen(true)] out string? value) => TryGetEnvVar(nameof(DevelopmentDb), out value);
    
    public string DevelopmentApi => GetRequiredEnvVar(nameof(DevelopmentApi));
    public bool TryGetDevelopmentApi([NotNullWhen(true)] out string? value) => TryGetEnvVar(nameof(DevelopmentApi), out value);
    
    public string SslCertLocation => GetRequiredEnvVar(nameof(SslCertLocation));
    public bool TryGetSslCertLocation([NotNullWhen(true)] out string? value) => TryGetEnvVar(nameof(SslCertLocation), out value);

    public string SslCertPassword => GetRequiredEnvVar(nameof(SslCertPassword));
    public bool TryGetSslCertPassword([NotNullWhen(true)] out string? value) => TryGetEnvVar(nameof(SslCertPassword), out value);


    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static ArgumentException NotSet(string env) => new($"Environment variable * {env} * was not set");
    private string GetRequiredEnvVar(string env) => configuration[env] ?? throw NotSet(env);
    private bool TryGetEnvVar(string env, [NotNullWhen(true)] out string? value) {
        if (configuration[env] is not {} v) {
            value = null;
            return false;
        }
        value = v;
        return true;
    }
}
