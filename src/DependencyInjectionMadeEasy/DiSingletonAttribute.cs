// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionMadeEasy;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[AttributeUsage(AttributeTargets.Class)]
public class DiSingletonAttribute(Type? instanceType = null) : AbstractServiceAttribute{
    public override Type? InstanceType => instanceType;
    public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class DiSingletonAttribute<T>() : DiSingletonAttribute(typeof(T));