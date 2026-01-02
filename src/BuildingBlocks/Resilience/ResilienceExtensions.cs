using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System.Net;

namespace BuildingBlocks.Resilience;

public static class ResilienceExtensions
{
    public static IServiceCollection AddResilientHttpClients(
        this IServiceCollection services)
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromSeconds(1),
            retryCount: 3);

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(delay, onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"Delaying for {timespan.TotalSeconds}s, then making retry {retryAttempt}");
            });

        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, breakDelay) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {breakDelay.TotalSeconds}s due to: {result.Exception?.Message ?? result.Result?.StatusCode.ToString()}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                });

        services.AddHttpClient("ResilientClient")
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy)
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }

    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetryAttempts = 3)
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromSeconds(1),
            retryCount: maxRetryAttempts);

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(delay, (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
            });

        return await retryPolicy.ExecuteAsync(operation);
    }
}
