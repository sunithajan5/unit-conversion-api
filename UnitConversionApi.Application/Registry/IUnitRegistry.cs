using System.Diagnostics.CodeAnalysis;
using UnitConversionApi.Application.Domain;

namespace UnitConversionApi.Application.Registry;

/// <summary>
/// Source of unit definitions.
/// </summary>
public interface IUnitRegistry
{
    /// <summary>
    /// Looks up a unit by its code.
    /// </summary>
    bool TryGetUnit(string code, [NotNullWhen(true)] out Unit? unit);

    IReadOnlyCollection<Unit> GetAll();
}