// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace NovaLab.Services.Twitch;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public abstract class AbstractScopedProcessor(IServiceScope scope) {
    protected T GetRequiredService<T>() where T : notnull => scope.ServiceProvider.GetRequiredService<T>();
}