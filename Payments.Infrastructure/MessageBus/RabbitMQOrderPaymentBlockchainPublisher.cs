using System;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Payments.Domain.Interfaces;
using Payments.Domain.Entities;
using RabbitMQ.Client.Events;

namespace Payments.Infrastructure.MessageBus;

public class RabbitMQOrderPaymentBlockchainPublisher : IOrderPaymentBlockchainPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQOrderPaymentBlockchainPublisher> _logger;
    private const string QueueName = "order_payment_blockchain_queue";
    private const string ExchangeName = "order_payment_blockchain_exchange";
    private const string RoutingKey = "blockchain.order_payment";
    private bool _disposed;

    public RabbitMQOrderPaymentBlockchainPublisher(
        IConfiguration configuration,
        ILogger<RabbitMQOrderPaymentBlockchainPublisher> logger)
    {
        _logger = logger;

        try
        {
            var connectionString = configuration.GetConnectionString("RabbitMQ") ?? "amqp://localhost";
            var factory = new ConnectionFactory 
            { 
                Uri = new Uri(connectionString),
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

            _logger.LogInformation("RabbitMQ Order Payment Blockchain Publisher connection established successfully");

            // Setup connection recovery event handlers
            _connection.ConnectionShutdown += OnConnectionShutdown;
            _connection.ConnectionBlocked += OnConnectionBlocked;
            _connection.ConnectionUnblocked += OnConnectionUnblocked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish RabbitMQ connection for order payment blockchain publisher");
            throw;
        }
    }

    public async Task PublishOrderPaymentBlockchainAsync(PublishBlockChain publishBlockChain)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMQOrderPaymentBlockchainPublisher));
        }

        try
        {
            // Check if channel is still open
            if (!_channel.IsOpen)
            {
                _logger.LogWarning("RabbitMQ channel is closed, cannot publish blockchain data");
                return;
            }

            var message = JsonSerializer.SerializeToUtf8Bytes(publishBlockChain);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.ContentType = "application/json";
            properties.Type = "OrderPaymentBlockchain";

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                basicProperties: properties,
                body: message
            );

            _logger.LogInformation(
                "Published combined order-payment blockchain data for order {OrderId} and payment {PaymentId}", 
                publishBlockChain.OrderId, publishBlockChain.PaymentId);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to publish combined order-payment blockchain data for order {OrderId} and payment {PaymentId}", 
                publishBlockChain.OrderId, publishBlockChain.PaymentId);
            throw;
        }
    }

    private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("RabbitMQ Order Payment Blockchain Publisher connection shutdown. Reason: {0}", e.ReplyText);
    }

    private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        _logger.LogWarning("RabbitMQ Order Payment Blockchain Publisher connection blocked. Reason: {0}", e.Reason);
    }

    private void OnConnectionUnblocked(object sender, EventArgs e)
    {
        _logger.LogInformation("RabbitMQ Order Payment Blockchain Publisher connection unblocked");
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
                    _logger.LogInformation("RabbitMQ Order Payment Blockchain Publisher channel closed");
                }
                _channel?.Dispose();

                if (_connection?.IsOpen ?? false)
                {
                    _connection.Close();
                    _logger.LogInformation("RabbitMQ Order Payment Blockchain Publisher connection closed");
                }
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RabbitMQ Order Payment Blockchain Publisher cleanup");
            }
        }

        _disposed = true;
    }

    ~RabbitMQOrderPaymentBlockchainPublisher()
    {
        Dispose(false);
    }
} 