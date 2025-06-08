using Mango.MessagePublisher.Services;
using Microsoft.AspNetCore.Mvc;
using static Mango.MessagePublisher.Models.MessageBroker;

namespace Mango.MessagePublisher.Controllers
{
    [ApiController]
    [Route("api/messageproducer")]
    public class MessageController : Controller
    {
        private readonly IRabbitPublisher _producer;

        public MessageController(IRabbitPublisher producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public IActionResult Send([FromBody] EmailMessage message)
        {
            _producer.PublishMessageAsync(message, RabbitMQQueues.EmailQueue);
            return Ok("Message sent to queue.");
        }
    }
}
