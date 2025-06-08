namespace Mango.MessagePublisher.Models
{
    public class MessageBroker
    {
        public class RabbitMQSetting
        {
            public string? HostName { get; set; }
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        //RabbitMQ Queue name
        public static class RabbitMQQueues
        {
            public const string EmailQueue = "EmailQueue";
        }

        public class EmailMessage
        {
            public string? ToEmail { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
        }
    }
}
