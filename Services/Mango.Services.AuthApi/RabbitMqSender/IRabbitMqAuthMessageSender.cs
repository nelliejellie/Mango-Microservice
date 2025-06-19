namespace Mango.Services.AuthApi.RabbitMqSender
{
    public interface IRabbitMqAuthMessageSender
    {
         Task SendMessage(object message, string queueName);
    }
}
