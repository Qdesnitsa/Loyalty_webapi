using System.Reflection;
using System.Text.Json.Serialization;
using Loyalty.Api.Authentication;
using Loyalty.Api.ExceptionHandling;
using Loyalty.Application;
using Loyalty.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("app", "loyalty")
        .Enrich.WithProperty("v", Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3))
#if DEBUG
        .WriteTo.Console()
        .MinimumLevel.Verbose();
#else
        .WriteTo.Console(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter());
#endif
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection(ApiKeyOptions.SectionName));
builder.Services
    .AddAuthentication(ApiKeyOptions.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyOptions.SchemeName, null);
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
