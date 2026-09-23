using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversionApi.Application.Contracts;
using UnitConversionApi.Application.Domain;

namespace UnitConversionApi.Tests;

// Calls the real API end to end, using an in-memory test server.
[TestClass]
public sealed class ApiIntegrationTests
{
    private static WebApplicationFactory<Program> _factory = null!;
    private static HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [TestMethod]
    public async Task Convert_ValidRequest_Returns200WithResult()
    {
        var response = await _client.GetAsync("/api/conversions?value=100&fromUnit=C&toUnit=F");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.IsNotNull(body);
        Assert.AreEqual(212.0, body.Result, 1e-9);
        Assert.AreEqual(UnitCategory.Temperature, body.Category);
    }

    [TestMethod]
    [DataRow("/api/conversions?value=1&fromUnit=xyz&toUnit=m", "Unknown unit")]
    [DataRow("/api/conversions?value=1&fromUnit=m&toUnit=kg", "Incompatible units")]
    [DataRow("/api/conversions?value=-300&fromUnit=C&toUnit=K", "Invalid value")]
    public async Task Convert_BadRequest_ReturnsProblemDetails(string url, string expectedTitle)
    {
        var response = await _client.GetAsync(url);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.AreEqual(expectedTitle, problem?.Title);
    }

    [TestMethod]
    public async Task Convert_MissingValue_ReturnsValidationError()
    {
        var response = await _client.GetAsync("/api/conversions?fromUnit=m&toUnit=ft");

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.IsNotNull(problem);
        Assert.IsTrue(problem.Errors.ContainsKey("Value"));
    }

    [TestMethod]
    public async Task GetUnits_WithCategory_ReturnsOnlyThatCategory()
    {
        var units = await _client.GetFromJsonAsync<List<UnitResponse>>("/api/units?category=Temperature");

        Assert.IsNotNull(units);
        Assert.HasCount(3, units);
        Assert.IsTrue(units.All(u => u.Category == UnitCategory.Temperature));
    }

    [TestMethod]
    public async Task GetUnits_InvalidCategory_Returns400()
    {
        var response = await _client.GetAsync("/api/units?category=Colour");

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
