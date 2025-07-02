using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Communication.Infrastructure.Configuration;

namespace Communication.Infrastructure.HealthChecks;

public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly MessageBusSettings _settings;

    public RabbitMQHealthCheck(IOptions<MessageBusSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_settings.Uri),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Try to declare a temporary queue to verify connection
            var tempQueueName = $"health_check_{Guid.NewGuid()}";
            channel.QueueDeclare(
                queue: tempQueueName,
                durable: false,
                exclusive: true,
                autoDelete: true,
                arguments: null);

            // Clean up the temporary queue
            channel.QueueDelete(tempQueueName);

            return await Task.FromResult(
                HealthCheckResult.Healthy("RabbitMQ connection is healthy"));
        }
        catch (Exception ex)
        {
            return await Task.FromResult(
                HealthCheckResult.Unhealthy("RabbitMQ connection is unhealthy", ex));
        }
    }
} 