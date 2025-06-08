using Mango.MessageBroker.Models;

namespace Mango.MessagePublisher.Services
{
    public interface IRabbitPublisher
    {
        Task PublishMessageAsync(object message, string queueName);
    }
}
