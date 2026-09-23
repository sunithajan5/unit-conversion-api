using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Application.Exceptions;

namespace UnitConversionApi.Api.ErrorHandling;

// Turns exceptions into ProblemDetails responses, so every error has the same shape
// and controllers don't need try/catch.
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            UnknownUnitException => BadRequest("Unknown unit", exception.Message),
            IncompatibleUnitsException => BadRequest("Incompatible units", exception.Message),
            InvalidValueException => BadRequest("Invalid value", exception.Message),
            _ => null
        };

        if (problem is null)
        {
            // Something we didn't expect. Log the details but don't send them to the caller.
            logger.LogError(exception, "Unhandled exception for {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);

            problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred"
            };
        }

        httpContext.Response.StatusCode = problem.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem
        });
    }

    private static ProblemDetails BadRequest(string title, string detail) => new()
    {
        Status = StatusCodes.Status400BadRequest,
        Title = title,
        Detail = detail
    };
}
