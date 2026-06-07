using FluentValidation;
using Loyalty.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Loyalty.Application;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ServicesCollectionExtensions).Assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(ServicesCollectionExtensions).Assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
