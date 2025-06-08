using Mango.Services.EmailApi.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.EmailApi.Messaging
{
    public class RabbitMQBusConsumer : BackgroundService
    {
        private readonly RabbitMqSettings _settings;
        private IConnection _connection;
        private IChannel? _channel;

        public  RabbitMQBusConsumer(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                Port = _settings.Port
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _settings.EmailShoppingCartQueue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            await _channel.QueueDeclareAsync(
                queue: _settings.RegisterUserQueue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[EmailAPI] Received: {message}");
                Console.WriteLine($"[EmailAPI] Sending Email: {message}");

                _channel.BasicAckAsync(args.DeliveryTag, false);
                await Task.CompletedTask;
            };

            _channel.BasicConsumeAsync(queue: _settings.EmailShoppingCartQueue, autoAck: false, consumer: consumer);
            _channel.BasicConsumeAsync(queue: _settings.RegisterUserQueue, autoAck: false, consumer: consumer);


            // Keep the background service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
