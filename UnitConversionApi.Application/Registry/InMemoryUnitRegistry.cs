using System.Diagnostics.CodeAnalysis;
using UnitConversionApi.Application.Domain;

namespace UnitConversionApi.Application.Registry;

/// <summary>
/// Hardcoded unit definitions.
/// </summary>
public sealed class InMemoryUnitRegistry : IUnitRegistry
{
    private readonly IReadOnlyCollection<Unit> _allUnits;
    private readonly IReadOnlyDictionary<string, Unit> _unitsByCode;

    public InMemoryUnitRegistry()
    {
        var units = CreateUnits();

        _allUnits = units.AsReadOnly();

        _unitsByCode = units.ToDictionary(u => u.Code, StringComparer.Ordinal);
    }

    public bool TryGetUnit(string code, [NotNullWhen(true)] out Unit? unit) =>
        _unitsByCode.TryGetValue(code, out unit);

    public IReadOnlyCollection<Unit> GetAll() => _allUnits;

    private static List<Unit> CreateUnits() =>
    [
        // Length
        Length("m", "Metre", 1),
        Length("cm", "Centimetre", 0.01),
        Length("mm", "Millimetre", 0.001),
        Length("km", "Kilometre", 1000),
        Length("in", "Inch", 0.0254),
        Length("ft", "Foot", 0.3048),
        Length("yd", "Yard", 0.9144),
        Length("mi", "Mile", 1609.344),

        // Weight
        Weight("kg", "Kilogram", 1),
        Weight("g", "Gram", 0.001),
        Weight("mg", "Milligram", 0.000001),
        Weight("t", "Tonne", 1000),
        Weight("lb", "Pound", 0.45359237),
        Weight("oz", "Ounce", 0.028349523125),
        Weight("st", "Stone", 6.35029318),

        // Temperature
        Temperature("K", "Kelvin", factor: 1, offset: 0),
        Temperature("C", "Celsius", factor: 1, offset: 273.15),
        Temperature("F", "Fahrenheit", factor: 5.0 / 9.0, offset: 273.15 - 32 * 5.0 / 9.0),
    ];

    private static Unit Length(string code, string name, double factor) =>
        new() { Code = code, Name = name, Category = UnitCategory.Length, Factor = factor };

    private static Unit Weight(string code, string name, double factor) =>
        new() { Code = code, Name = name, Category = UnitCategory.Weight, Factor = factor };

    private static Unit Temperature(string code, string name, double factor, double offset) =>
        new() { Code = code, Name = name, Category = UnitCategory.Temperature, Factor = factor, Offset = offset };
}