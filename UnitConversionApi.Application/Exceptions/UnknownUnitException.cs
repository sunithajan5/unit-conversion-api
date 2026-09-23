namespace UnitConversionApi.Application.Exceptions;

// Thrown when a unit code is not in the registry.
public sealed class UnknownUnitException(string unitCode)
    : ConversionException($"Unit '{unitCode}' is not supported. Codes are case sensitive.")
{
    public string UnitCode { get; } = unitCode;
}
