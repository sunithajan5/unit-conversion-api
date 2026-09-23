using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Application.Contracts;
using UnitConversionApi.Application.Domain;
using UnitConversionApi.Application.Registry;

namespace UnitConversionApi.Api.Controllers;

[ApiController]
[Route("api/units")]
public class UnitsController(IUnitRegistry unitRegistry) : ControllerBase
{
    // GET /api/units or /api/units?category=Length
    [HttpGet]
    [ProducesResponseType<IEnumerable<UnitResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<UnitResponse>> GetUnits([FromQuery] UnitCategory? category)
    {
        var units = unitRegistry.GetAll()
            .Where(u => category is null || u.Category == category)
            .Select(u => new UnitResponse(u.Code, u.Name, u.Category));

        return Ok(units);
    }
}
