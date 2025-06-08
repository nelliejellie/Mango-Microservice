
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
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            var props = new BasicProperties();
            await channel.BasicPublishAsync(exchange:"",routingKey:queueName, false, props, body);

        }
    }
}
