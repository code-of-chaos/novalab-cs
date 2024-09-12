// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NovaLab.Server.Database.Converters;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UlidToStringConverter(ConverterMappingHints? mappingHints = null) : ValueConverter<Ulid, string>(
    convertToProviderExpression: x => x.ToString(),
    convertFromProviderExpression: x => Ulid.Parse(x),
    mappingHints: DefaultHints.With(mappingHints)
) {
    [UsedImplicitly] public UlidToStringConverter() : this(null) { }
    private static readonly ConverterMappingHints DefaultHints = new(size: 26);
}
