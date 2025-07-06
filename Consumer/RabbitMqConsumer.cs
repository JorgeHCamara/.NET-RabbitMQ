using Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Consumer
{
    public class RabbitMqConsumer : IHostedService, IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _queueName = "my-queue";

        public RabbitMqConsumer(IConfiguration configuration, ILogger<RabbitMqConsumer> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection(new[] { factory.HostName });
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(_queueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var order = JsonSerializer.Deserialize<Order>(message);

                    if (order != null && order.User != null)
                    {
                        _logger.LogInformation("Order received: {Order}", order.ToString());
                    }
                    else
                    {
                        _logger.LogInformation("Default message received: {Message}", message);
                    }
                }
                catch (JsonException)
                {
                    _logger.LogInformation("Unstructured message received: {Message}", message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing received message: {Message}", message);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
