namespace UnitConversionApi.Application.Exceptions;

// Base class for errors caused by a bad request. The API returns these as 400.
public abstract class ConversionException(string message) : Exception(message);
