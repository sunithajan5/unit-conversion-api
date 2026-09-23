using UnitConversionApi.Api.ErrorHandling;
using UnitConversionApi.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddControllers();

// All errors (validation, our own exceptions, anything unexpected) come back as ProblemDetails.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Swagger UI on top of the built-in OpenAPI document, for trying the API in a browser.
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Unit Conversion API"));
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
