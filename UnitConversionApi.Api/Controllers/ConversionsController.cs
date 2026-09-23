using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Application.Contracts;
using UnitConversionApi.Application.Services;

namespace UnitConversionApi.Api.Controllers;

[ApiController]
[Route("api/conversions")]
public class ConversionsController(IConversionService conversionService) : ControllerBase
{
    // GET /api/conversions?value=100&fromUnit=C&toUnit=F
   
    [HttpGet]
    [ProducesResponseType<ConversionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ConversionResponse> Convert([FromQuery] ConversionRequest request)
    {
        // [ApiController] has already returned 400 if Value was missing.
        // Errors thrown by the service are handled by GlobalExceptionHandler.
        var response = conversionService.Convert(request.Value!.Value, request.FromUnit, request.ToUnit);

        return Ok(response);
    }
}
