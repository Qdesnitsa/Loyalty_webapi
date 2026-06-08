using Loyalty.Api.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Loyalty.Api.Infrastructure;

public sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Loyalty Api",
            Version = "v1",
            Description = "Loyalty participation API"
        });

        options.OperationFilter<SwaggerRouteParameterFilter>();
        options.OperationFilter<SwaggerDefaultValuesFilter>();

        var xmlPath = Path.Combine(AppContext.BaseDirectory, "Loyalty.Api.xml");
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        var apiKeyScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyOptions.HeaderName,
            Description = "Loyalty API key",
            Reference = new OpenApiReference
            {
                Id = ApiKeyOptions.SchemeName,
                Type = ReferenceType.SecurityScheme
            }
        };

        options.AddSecurityDefinition(apiKeyScheme.Reference!.Id, apiKeyScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { apiKeyScheme, Array.Empty<string>() }
        });
    }
}
