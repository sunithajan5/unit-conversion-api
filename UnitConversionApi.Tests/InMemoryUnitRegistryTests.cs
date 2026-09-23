using UnitConversionApi.Application.Domain;
using UnitConversionApi.Application.Registry;

namespace UnitConversionApi.Tests;

[TestClass]
public sealed class InMemoryUnitRegistryTests
{
    private readonly InMemoryUnitRegistry _registry = new();

    [TestMethod]
    public void AllUnits_HaveNonZeroFactor()
    {
        // Converting out of the base unit divides by Factor, so zero would break every conversion.
        foreach (var unit in _registry.GetAll())
        {
            Assert.AreNotEqual(0.0, unit.Factor, $"{unit.Code} has a factor of zero");
        }
    }

    [TestMethod]
    [DataRow(UnitCategory.Length, "m")]
    [DataRow(UnitCategory.Weight, "kg")]
    [DataRow(UnitCategory.Temperature, "K")]
    public void EachCategory_HasExactlyOneBaseUnit(UnitCategory category, string expectedBaseCode)
    {
        var baseUnits = _registry.GetAll()
            .Where(u => u.Category == category && u.Factor == 1 && u.Offset == 0)
            .ToList();

        Assert.HasCount(1, baseUnits);
        Assert.AreEqual(expectedBaseCode, baseUnits[0].Code);
    }

    [TestMethod]
    public void TryGetUnit_KnownCode_ReturnsUnit()
    {
        var found = _registry.TryGetUnit("km", out var unit);

        Assert.IsTrue(found);
        Assert.IsNotNull(unit);
        Assert.AreEqual("Kilometre", unit.Name);
        Assert.AreEqual(UnitCategory.Length, unit.Category);
    }

    [TestMethod]
    public void TryGetUnit_UnknownCode_ReturnsFalse()
    {
        var found = _registry.TryGetUnit("furlong", out var unit);

        Assert.IsFalse(found);
        Assert.IsNull(unit);
    }

    [TestMethod]
    [DataRow("MM")]
    [DataRow("Km")]
    [DataRow("c")]
    public void TryGetUnit_IsCaseSensitive(string code)
    {
        Assert.IsFalse(_registry.TryGetUnit(code, out _));
    }
}
