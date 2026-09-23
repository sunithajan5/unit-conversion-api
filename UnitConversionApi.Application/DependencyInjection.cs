using Microsoft.Extensions.DependencyInjection;
using UnitConversionApi.Application.Registry;

namespace UnitConversionApi.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the application layer services. 
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
      
        services.AddSingleton<IUnitRegistry, InMemoryUnitRegistry>();

        return services;
    }

}