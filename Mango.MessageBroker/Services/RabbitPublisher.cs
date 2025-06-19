
using Mango.MessageBroker.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace Mango.MessagePublisher.Services
{
    public class RabbitPublisher : IRabbitPublisher
    {
        // one publisher to many consumers
        public async Task PublishFanOutMessageAsync(object message, string exchangeName, string routingKey = "")
        {
            var settings = new RabbitMqSettings();
            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password
            };

            await using var conn = await factory.CreateConnectionAsync();
            await using var channel = await conn.CreateChannelAsync();

            // Declare the fanout exchange (idempotent operation)
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false
            );

            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            var props = new BasicProperties();

            // Routing key is ignored in fanout exchange
            await channel.BasicPublishAsync(exchange: exchangeName, routingKey: "", mandatory: false, props, body);
        }

        public async Task PublishMessageAsync(object message, string queueName)
        {
            var settings = new RabbitMqSettings();
            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password
            };

            IConnection conn = await factory.CreateConnectionAsync();
            IChannel channel = await conn.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: queueName);
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            var props = new BasicProperties();
            await channel.BasicPublishAsync(exchange:"",routingKey:queueName, false, props, body);
            
        }
    }
}
