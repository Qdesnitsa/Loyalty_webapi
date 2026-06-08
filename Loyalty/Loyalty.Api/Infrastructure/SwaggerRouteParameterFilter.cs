using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Loyalty.Api.Infrastructure;

/// <summary>Removes route parameters that are not present in the route template from Swagger operations</summary>
public sealed class SwaggerRouteParameterFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var routeParameters = context.MethodInfo.GetParameters()
            .Where(param => param.GetCustomAttribute<FromRouteAttribute>() != null && param.Name != null)
            .Select(param => param.Name!);

        foreach (var paramName in routeParameters)
        {
            var parameter = operation.Parameters.FirstOrDefault(
                p => p.In == ParameterLocation.Path && p.Name == paramName);

            if (parameter != null
                && context.ApiDescription.RelativePath is { } relativePath
                && !relativePath.Contains(paramName, StringComparison.OrdinalIgnoreCase))
            {
                operation.Parameters.Remove(parameter);
            }
        }
    }
}
