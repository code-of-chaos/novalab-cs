// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionMadeEasy;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DiEasifier(IServiceCollection serviceCollection) {
    public void AssignAll() {
        IEnumerable<Type> types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .AsQueryable();
        
        _AssignTo(types, typeof(DiSingletonAttribute));
        _AssignTo(types, typeof(DiTransientAttribute));
        _AssignTo(types, typeof(DiScopedAttribute));
    }

    private void _AssignTo(IEnumerable<Type> types, Type attributeType) {
        types
            .Select(type => new {type, attrib = type.GetCustomAttributes(attributeType)})
            .ToList()
            .ForEach(box => {
                foreach (Attribute baseAttrib in box.attrib) {
                    var attribute = (AbstractServiceAttribute)baseAttrib;

                    serviceCollection.Add(attribute.InstanceType is null
                        ? new ServiceDescriptor(box.type, box.type, attribute.ServiceLifetime)
                        : new ServiceDescriptor(box.type, attribute.InstanceType, attribute.ServiceLifetime));
                }

            });
    }
}