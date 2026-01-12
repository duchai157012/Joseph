using BuildingBlocks.Resilience;
using FluentValidation;
using MediatR;
using Order.Application.Common.Behaviors;
using Order.Application.Common.Interfaces;
using Order.Application.Interfaces;
using Order.Infrastructure.Clients;
using Order.Infrastructure.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System.Net;

namespace Order.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(Order.Application.Features.Orders.Commands.CreateOrder.CreateOrderCommand).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        AddResilientCatalogClient(services, configuration);

        return services;
    }

    private static void AddResilientCatalogClient(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var catalogBaseUrl = configuration["Services:CatalogApi:BaseUrl"] 
            ?? "http://localhost:5000";

        var delay = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromSeconds(1),
            retryCount: 3);

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(delay, onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine(
                    $"[CatalogClient] Retry {retryAttempt} after {timespan.TotalSeconds:F2}s");
            });

        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, breakDelay) =>
                {
                    Console.WriteLine(
                        $"[CatalogClient] Circuit breaker opened for {breakDelay.TotalSeconds}s");
                },
                onReset: () =>
                {
                    Console.WriteLine("[CatalogClient] Circuit breaker reset");
                });

        services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(client =>
            {
                client.BaseAddress = new Uri(catalogBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy)
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));
    }
}
