using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBroker.Models
{
    public class RabbitMqSettings
    {
        public string? HostName { get; set; } = "localhost";
        public string? UserName { get; set; } = "guest";
        public string? Password { get; set; } = "guest";
    }
}
