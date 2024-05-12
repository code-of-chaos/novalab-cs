// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace NovaLab.Services;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public abstract class AbstractServiceCollection(IServiceCollection serviceCollection) {
    protected IServiceCollection _serviceCollection = serviceCollection;
    
    public abstract void DefineServices();
}