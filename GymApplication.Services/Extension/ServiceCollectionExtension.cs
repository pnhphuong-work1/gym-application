using FluentValidation;
using GymApplication.Services.Behaviors;
using GymApplication.Services.Mapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GymApplication.Services.Extension;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServicesLayer(this IServiceCollection services)
    {
        return services
            .AddMediatR()
            .AddValidators()
            .AddAutoMapper();
    }
    
    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cof =>
            {
                cof.RegisterServicesFromAssembly(AssemblyRef.ServicesAssembly);
            })
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        return services;
    }
    
    private static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ServiceProfile));
        return services;
    }
    
    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Shared.AssemblyRef.SharedAssembly, includeInternalTypes: true);
        return services;
    }
}