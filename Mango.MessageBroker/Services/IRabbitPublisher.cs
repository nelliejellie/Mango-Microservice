﻿using Mango.MessageBroker.Models;

namespace Mango.MessagePublisher.Services
{
    public interface IRabbitPublisher
    {
        Task PublishMessageAsync(object message, string queueName);
        Task PublishFanOutMessageAsync(object message, string exchangeName, string routingKey = "");
    }
}
