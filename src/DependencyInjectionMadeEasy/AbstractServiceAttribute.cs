// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionMadeEasy;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public abstract class AbstractServiceAttribute : Attribute {
    public abstract Type? InstanceType { get; }
    public abstract ServiceLifetime ServiceLifetime { get; }
}