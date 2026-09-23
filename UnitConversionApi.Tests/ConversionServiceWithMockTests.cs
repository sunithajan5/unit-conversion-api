using Moq;
using UnitConversionApi.Application.Domain;
using UnitConversionApi.Application.Exceptions;
using UnitConversionApi.Application.Registry;
using UnitConversionApi.Application.Services;

namespace UnitConversionApi.Tests;

// Uses a fake registry to check the service only relies on IUnitRegistry.
[TestClass]
public sealed class ConversionServiceWithMockTests
{
    private readonly Mock<IUnitRegistry> _registry = new();

    public ConversionServiceWithMockTests()
    {
        // Anything not set up below is treated as unknown.
        Unit? notFound = null;
        _registry.Setup(r => r.TryGetUnit(It.IsAny<string>(), out notFound)).Returns(false);
    }

    [TestMethod]
    public void Convert_UsesUnitDefinitionsFromRegistry()
    {
        // "hand" is not in the real unit list, it only exists in this mock.
        AddUnit(new Unit { Code = "hand", Name = "Hand", Category = UnitCategory.Length, Factor = 0.1016 });
        AddUnit(new Unit { Code = "m", Name = "Metre", Category = UnitCategory.Length, Factor = 1 });

        var service = new ConversionService(_registry.Object);

        var response = service.Convert(10, "hand", "m");

        Assert.AreEqual(1.016, response.Result, 1e-9);
    }

    [TestMethod]
    public void Convert_UnitMissingFromRegistry_ThrowsUnknownUnit()
    {
        AddUnit(new Unit { Code = "m", Name = "Metre", Category = UnitCategory.Length, Factor = 1 });

        var service = new ConversionService(_registry.Object);

        Assert.ThrowsExactly<UnknownUnitException>(() => service.Convert(1, "m", "ft"));
    }

    private void AddUnit(Unit unit)
    {
        Unit? result = unit;
        _registry.Setup(r => r.TryGetUnit(unit.Code, out result)).Returns(true);
    }
}
