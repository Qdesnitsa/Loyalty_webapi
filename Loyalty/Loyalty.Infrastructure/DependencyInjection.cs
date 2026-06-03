using Loyalty.Application.Abstractions;
using Loyalty.Infrastructure.Data;
using Loyalty.Infrastructure.Health;
using Loyalty.Infrastructure.Messaging;
using Loyalty.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Loyalty.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongo = configuration.GetSection(MongoOptions.SectionName).Get<MongoOptions>()
            ?? throw new InvalidOperationException($"Configuration section '{MongoOptions.SectionName}' is missing.");

        if (string.IsNullOrWhiteSpace(mongo.ConnectionString))
            throw new InvalidOperationException("Mongo:ConnectionString is not configured.");

        if (string.IsNullOrWhiteSpace(mongo.Database))
            throw new InvalidOperationException("Mongo:Database is not configured.");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongo.ConnectionString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongo.Database));

        services.AddHealthChecks()
            .AddCheck<MongoHealthCheck>("mongodb");

        services.AddScoped<IProgramRepository, ProgramRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IParticipationRepository, ParticipationRepository>();

        services.Configure<KafkaConsumerConfig>(configuration.GetSection(KafkaConsumerConfig.SectionName));
        services.AddHostedService<TransactionCreatedConsumer>();

        return services;
    }
}
