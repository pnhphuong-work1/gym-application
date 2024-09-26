using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GymApplication.api.Attribute;

public class ReplaceVersionWithExactValueInPathFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var updatedPaths = new OpenApiPaths();

        foreach (var (key, value) in swaggerDoc.Paths)
        {
            var index = key.IndexOf("api/v", System.StringComparison.Ordinal);
            var newKey = key.Insert(index + 5, swaggerDoc.Info.Version);
            updatedPaths.Add(newKey, value);
        }

        swaggerDoc.Paths = updatedPaths; // Assign the new OpenApiPaths object
    }
}