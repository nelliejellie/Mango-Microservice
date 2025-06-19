using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.AuthApi.RabbitMqSender
{
    public class RabbitMqAuthMessageSender : IRabbitMqAuthMessageSender
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;

        public RabbitMqAuthMessageSender()
        {
            _hostname = "localhost";
            _password = "guest";
            _username = "guest";
        }
        public async Task SendMessage(object message, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            _connection = await factory.CreateConnectionAsync();
            IChannel channel = await _connection.CreateChannelAsync();
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            var props = new BasicProperties();
            await channel.QueueDeclareAsync(queue: queueName,false,false,false,null);
            await channel.BasicPublishAsync(exchange: "", routingKey: queueName, false, props, body);
        }
    }
}
