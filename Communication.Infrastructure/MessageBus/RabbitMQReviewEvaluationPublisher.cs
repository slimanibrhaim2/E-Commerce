using System;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Communication.Domain.Interfaces;
using Communication.Domain.Entities;
using Communication.Infrastructure.Configuration;
using RabbitMQ.Client.Events;

namespace Communication.Infrastructure.MessageBus;

public class RabbitMQReviewEvaluationPublisher : IReviewEvaluationPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQReviewEvaluationPublisher> _logger;
    private const string QueueName = "review_evaluation_queue";
    private const string ExchangeName = "review_evaluation_exchange";
    private const string RoutingKey = "review.evaluation";
    private bool _disposed;

    public RabbitMQReviewEvaluationPublisher(
        IOptions<MessageBusSettings> settings,
        ILogger<RabbitMQReviewEvaluationPublisher> logger)
    {
        _logger = logger;

        try
        {
            var factory = new ConnectionFactory 
            { 
                Uri = new Uri(settings.Value.Uri),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);
            
            // Declare queue
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Bind queue to exchange
            _channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingKey);

            _logger.LogInformation("RabbitMQ connection established successfully");

            // Setup connection recovery event handlers
            _connection.ConnectionShutdown += OnConnectionShutdown;
            _connection.ConnectionBlocked += OnConnectionBlocked;
            _connection.ConnectionUnblocked += OnConnectionUnblocked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish RabbitMQ connection");
            throw;
        }
    }

    public async Task PublishForEvaluationAsync(PublishReview review)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMQReviewEvaluationPublisher));
        }

        try
        {
            var message = JsonSerializer.SerializeToUtf8Bytes(review);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.ContentType = "application/json";
            properties.Type = "ReviewEvaluation";

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                basicProperties: properties,
                body: message
            );

            _logger.LogInformation(
                "Published review {ReviewId} for evaluation", 
                review.Id);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to publish review {ReviewId} for evaluation", 
                review.Id);
            throw;
        }
    }

    private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection shutdown. Reason: {0}", e.ReplyText);
    }

    private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection blocked. Reason: {0}", e.Reason);
    }

    private void OnConnectionUnblocked(object sender, EventArgs e)
    {
        _logger.LogInformation("RabbitMQ connection unblocked");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            try
            {
                if (_channel?.IsOpen ?? false)
                {
                    _channel.Close();
                    _logger.LogInformation("RabbitMQ channel closed");
                }
                _channel?.Dispose();

                if (_connection?.IsOpen ?? false)
                {
                    _connection.Close();
                    _logger.LogInformation("RabbitMQ connection closed");
                }
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RabbitMQ cleanup");
            }
        }

        _disposed = true;
    }

    ~RabbitMQReviewEvaluationPublisher()
    {
        Dispose(false);
    }
} 