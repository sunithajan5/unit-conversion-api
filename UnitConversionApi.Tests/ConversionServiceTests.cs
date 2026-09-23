using UnitConversionApi.Application.Domain;
using UnitConversionApi.Application.Exceptions;
using UnitConversionApi.Application.Registry;
using UnitConversionApi.Application.Services;

namespace UnitConversionApi.Tests;

// Runs conversions against the real unit list, so these also check the seed data.
[TestClass]
public sealed class ConversionServiceTests
{
    private const double Tolerance = 1e-9;

    private readonly ConversionService _service = new(new InMemoryUnitRegistry());

    [TestMethod]
    // Length
    [DataRow(1.0, "km", "m", 1000.0)]
    [DataRow(1.0, "ft", "m", 0.3048)]
    [DataRow(12.0, "in", "ft", 1.0)]
    [DataRow(1.0, "mi", "km", 1.609344)]
    [DataRow(250.0, "cm", "mm", 2500.0)]
    // Weight
    [DataRow(1.0, "lb", "kg", 0.45359237)]
    [DataRow(1.0, "st", "lb", 14.0)]
    [DataRow(16.0, "oz", "lb", 1.0)]
    [DataRow(1000.0, "mg", "g", 1.0)]
    // Temperature
    [DataRow(100.0, "C", "F", 212.0)]
    [DataRow(32.0, "F", "C", 0.0)]
    [DataRow(-40.0, "F", "C", -40.0)]
    [DataRow(98.6, "F", "C", 37.0)]
    [DataRow(0.0, "K", "C", -273.15)]
    [DataRow(-459.67, "F", "K", 0.0)]
    public void Convert_ReturnsExpectedResult(double value, string from, string to, double expected)
    {
        var response = _service.Convert(value, from, to);

        Assert.AreEqual(expected, response.Result, Tolerance);
    }

    [TestMethod]
    public void Convert_VerySmallResult_IsNotRoundedAway()
    {
        // 1 mg is a billionth of a tonne. Rounding by decimal places would turn this into 0.
        var response = _service.Convert(1, "mg", "t");

        Assert.AreEqual(1e-9, response.Result, 1e-18);
    }

    [TestMethod]
    public void Convert_ResponseEchoesRequestAndCategory()
    {
        var response = _service.Convert(5, "km", "mi");

        Assert.AreEqual(5.0, response.Value);
        Assert.AreEqual("km", response.FromUnit);
        Assert.AreEqual("mi", response.ToUnit);
        Assert.AreEqual(UnitCategory.Length, response.Category);
    }

    [TestMethod]
    [DataRow(5.5, "m")]
    [DataRow(212.0, "F")]
    [DataRow(-10.0, "C")]
    public void Convert_SameUnit_ReturnsValueUnchanged(double value, string unit)
    {
        var response = _service.Convert(value, unit, unit);

        Assert.AreEqual(value, response.Result);
    }

    [TestMethod]
    [DataRow(451.0, "F", "C")]
    [DataRow(3.7, "mi", "cm")]
    [DataRow(0.125, "oz", "kg")]
    public void Convert_RoundTrip_ReturnsOriginalValue(double value, string from, string to)
    {
        var there = _service.Convert(value, from, to);
        var back = _service.Convert(there.Result, to, from);

        Assert.AreEqual(value, back.Result, Tolerance);
    }

    [TestMethod]
    public void Convert_NegativeLength_IsAllowed()
    {
        var response = _service.Convert(-5, "m", "cm");

        Assert.AreEqual(-500.0, response.Result, Tolerance);
    }

    [TestMethod]
    [DataRow("m", "kg")]
    [DataRow("C", "ft")]
    [DataRow("lb", "K")]
    public void Convert_DifferentCategories_Throws(string from, string to)
    {
        Assert.ThrowsExactly<IncompatibleUnitsException>(() => _service.Convert(1, from, to));
    }

    [TestMethod]
    [DataRow("xyz", "m")]
    [DataRow("m", "xyz")]
    public void Convert_UnknownUnit_Throws(string from, string to)
    {
        var ex = Assert.ThrowsExactly<UnknownUnitException>(() => _service.Convert(1, from, to));

        Assert.AreEqual("xyz", ex.UnitCode);
    }

    [TestMethod]
    [DataRow(double.NaN)]
    [DataRow(double.PositiveInfinity)]
    [DataRow(double.NegativeInfinity)]
    public void Convert_NonFiniteValue_Throws(double value)
    {
        Assert.ThrowsExactly<InvalidValueException>(() => _service.Convert(value, "m", "ft"));
    }

    [TestMethod]
    [DataRow(-300.0, "C")]
    [DataRow(-1.0, "K")]
    [DataRow(-500.0, "F")]
    public void Convert_BelowAbsoluteZero_Throws(double value, string from)
    {
        Assert.ThrowsExactly<InvalidValueException>(() => _service.Convert(value, from, "K"));
    }
}
