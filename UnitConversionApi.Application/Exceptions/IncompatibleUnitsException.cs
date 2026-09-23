using UnitConversionApi.Application.Domain;

namespace UnitConversionApi.Application.Exceptions;

// Thrown when converting between different categories, e.g. metres to kilograms.
public sealed class IncompatibleUnitsException(Unit from, Unit to)
    : ConversionException(
        $"Cannot convert {from.Name} ({from.Category}) to {to.Name} ({to.Category}). " +
        "Both units must be in the same category.");
