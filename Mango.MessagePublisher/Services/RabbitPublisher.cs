
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;
using static Mango.MessagePublisher.Models.MessageBroker;

namespace Mango.MessagePublisher.Services
{
    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly RabbitMQSetting _rabbitMqSetting;

        public RabbitPublisher(IOptions<RabbitMQSetting> rabbitMqSetting)
        {
            _rabbitMqSetting = rabbitMqSetting.Value;
        }
        public async Task PublishMessageAsync(object message, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSetting.HostName,
                UserName = _rabbitMqSetting.UserName,
                Password = _rabbitMqSetting.Password
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
