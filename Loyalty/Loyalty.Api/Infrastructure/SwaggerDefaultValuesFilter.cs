using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Loyalty.Api.Infrastructure;

/// <summary>Applies ASP.NET Core parameter defaults and descriptions to Swagger operations</summary>
public sealed class SwaggerDefaultValuesFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in operation.Parameters)
        {
            var description = context.ApiDescription.ParameterDescriptions
                .FirstOrDefault(p => p.Name == parameter.Name);
            if (description is null)
            {
                continue;
            }

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null)
            {
                parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
            }
        }
    }
}
