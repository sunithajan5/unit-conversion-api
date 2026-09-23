using Microsoft.Extensions.DependencyInjection;
using UnitConversionApi.Application.Registry;
using UnitConversionApi.Application.Services;

namespace UnitConversionApi.Application;

public static class DependencyInjection
{
    // Registers everything the Application layer provides.
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Singletons, as neither holds any per-request state.
        services.AddSingleton<IUnitRegistry, InMemoryUnitRegistry>();
        services.AddSingleton<IConversionService, ConversionService>();

        return services;
    }
}