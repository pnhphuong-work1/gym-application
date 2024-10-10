using GymApplication.Shared.Attribute;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GymApplication.api.Attribute;

public class SwaggerIgnoreFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null)
            return;

        var excludedProperties = context.Type.GetProperties()
            .Where(prop => prop.GetCustomAttributes(typeof(SwaggerIgnoreAttribute), true).Any());

        foreach (var excludedProperty in excludedProperties)
        {
            var propertyName = schema.Properties.Keys
                .SingleOrDefault(x => x.ToLower() == excludedProperty.Name.ToLower());

            if (!string.IsNullOrEmpty(propertyName))
            {
                schema.Properties.Remove(propertyName);
            }
        }
    }
}
