using UnitConversionApi.Application.Domain;

namespace UnitConversionApi.Application.Contracts;

/// <summary>
/// Result of a conversion
/// </summary>
public record ConversionResponse(
    double Value,
    string FromUnit,
    string ToUnit,
    double Result,
    UnitCategory Category);