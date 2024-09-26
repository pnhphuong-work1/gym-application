using Asp.Versioning;
using GymApplication.api.Attribute;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GymApplication.api.Extension;

public static class SwaggerExtension
{
    public static void AddApiVersionForController(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
        }).AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'G";
                options.SubstituteApiVersionInUrl = true;
            });
    } 
}