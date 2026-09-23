namespace UnitConversionApi.Application.Exceptions;

// Thrown when the value itself can't be converted, e.g. NaN or below absolute zero.
public sealed class InvalidValueException(string message) : ConversionException(message);
