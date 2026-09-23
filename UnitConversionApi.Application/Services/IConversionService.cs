using UnitConversionApi.Application.Contracts;

namespace UnitConversionApi.Application.Services;

public interface IConversionService
{
    // Converts a value between two units of the same category.
    ConversionResponse Convert(double value, string fromUnitCode, string toUnitCode);
}
