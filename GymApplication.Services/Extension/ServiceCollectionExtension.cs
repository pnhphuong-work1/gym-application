using FluentValidation;
using GymApplication.Services.Abstractions;
using GymApplication.Services.Authentication;
using GymApplication.Services.Behaviors;
using GymApplication.Services.Caching;
using GymApplication.Services.Mapper;
using GymApplication.Services.Payment;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymApplication.Services.Extension;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServicesLayer(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddMediatR()
            .AddRedisCache(configuration)
            .AddValidators()
            .AddJwtService()
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
    
    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connectionString;
        });
        
        services.AddTransient<ICacheServices, CacheServices>();
        
        return services;
    }
    
    private static IServiceCollection AddJwtService(this IServiceCollection services)
    {
        services.AddTransient<IJwtServices, JwtServices>();
        services.AddScoped<IPayOsServices, PayOsServices>();
        return services;
    }
}