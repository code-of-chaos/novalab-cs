// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionMadeEasy;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[AttributeUsage(AttributeTargets.Class)]
public class DiScopedAttribute(Type? instanceType = null) : AbstractServiceAttribute{
    public override Type? InstanceType => instanceType;
    public override ServiceLifetime ServiceLifetime => ServiceLifetime.Scoped;
}

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class DiScopedAttribute<T>() : DiSingletonAttribute(typeof(T));