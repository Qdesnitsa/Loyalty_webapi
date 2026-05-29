using Loyalty.Application.Programs;
using Microsoft.Extensions.DependencyInjection;

namespace Loyalty.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateProgramService>();

        return services;
    }
}
