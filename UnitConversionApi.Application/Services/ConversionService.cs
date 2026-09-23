using System.Globalization;
using UnitConversionApi.Application.Contracts;
using UnitConversionApi.Application.Domain;
using UnitConversionApi.Application.Exceptions;
using UnitConversionApi.Application.Registry;

namespace UnitConversionApi.Application.Services;

public sealed class ConversionService(IUnitRegistry unitRegistry) : IConversionService
{
    private const int SignificantFigures = 12;

    // Anything smaller than this is floating point noise and is treated as zero.
    private const double ZeroTolerance = 1e-9;

    public ConversionResponse Convert(double value, string fromUnitCode, string toUnitCode)
    {
        if (!double.IsFinite(value))
        {
            throw new InvalidValueException("Value must be a finite number.");
        }

        var from = GetUnit(fromUnitCode);
        var to = GetUnit(toUnitCode);

        if (from.Category != to.Category)
        {
            throw new IncompatibleUnitsException(from, to);
        }

        // Step 1: convert into the base unit (metre, kilogram or kelvin).
        var baseValue = value * from.Factor + from.Offset;
        if (from.Offset != 0)
        {
            baseValue = SnapToZero(baseValue);
        }

        if (from.Category == UnitCategory.Temperature && baseValue < 0)
        {
            throw new InvalidValueException($"{value} {from.Code} is below absolute zero.");
        }

        // Step 2: convert from the base unit into the target unit.
        var shifted = baseValue - to.Offset;
        if (to.Offset != 0)
        {
            shifted = SnapToZero(shifted);
        }

        var result = shifted / to.Factor;

        return new ConversionResponse(value, from.Code, to.Code, RoundToSignificantFigures(result), from.Category);
    }

    private Unit GetUnit(string code) =>
        unitRegistry.TryGetUnit(code, out var unit) ? unit : throw new UnknownUnitException(code);

    private static double SnapToZero(double value) => Math.Abs(value) < ZeroTolerance ? 0 : value;

    // Significant figures rather than decimal places, so tiny results like 1 mg in tonnes aren't lost.
    private static double RoundToSignificantFigures(double value)
    {
        var rounded = value.ToString("G" + SignificantFigures, CultureInfo.InvariantCulture);
        return double.Parse(rounded, CultureInfo.InvariantCulture);
    }
}
