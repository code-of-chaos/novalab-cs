// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionMadeEasy;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[AttributeUsage(AttributeTargets.Class)]
public class DiTransientAttribute(Type? instanceType = null) : AbstractServiceAttribute{
    public override Type? InstanceType => instanceType;
    public override ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
}

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class DiTransientAttribute<T>() : DiSingletonAttribute(typeof(T));