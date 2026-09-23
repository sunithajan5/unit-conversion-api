using UnitConversionApi.Application.Domain;

namespace UnitConversionApi.Application.Contracts;

/// <summary>
/// Public view of a unit.
/// </summary>
public record UnitResponse(string Code, string Name, UnitCategory Category);