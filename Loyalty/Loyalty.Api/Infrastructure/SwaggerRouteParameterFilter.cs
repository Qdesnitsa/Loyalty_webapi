using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Loyalty.Api.Infrastructure;

public sealed class SwaggerRouteParameterFilter : IOperationFilter
{
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
