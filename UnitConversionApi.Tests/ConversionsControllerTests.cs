using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitConversionApi.Api.Controllers;
using UnitConversionApi.Application.Contracts;
using UnitConversionApi.Application.Domain;
using UnitConversionApi.Application.Services;

namespace UnitConversionApi.Tests;

[TestClass]
public sealed class ConversionsControllerTests
{
    [TestMethod]
    public void Convert_PassesRequestToServiceAndReturnsOk()
    {
        var expected = new ConversionResponse(100, "C", "F", 212, UnitCategory.Temperature);
        var service = new Mock<IConversionService>();
        service.Setup(s => s.Convert(100, "C", "F")).Returns(expected);

        var controller = new ConversionsController(service.Object);

        var result = controller.Convert(new ConversionRequest(100, "C", "F"));

        var ok = result.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(expected, ok.Value);
        service.Verify(s => s.Convert(100, "C", "F"), Times.Once());
    }
}
