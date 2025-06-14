﻿namespace Mango.Services.EmailApi.Models
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string EmailShoppingCartQueue { get; set; }
        public string RegisterUserQueue { get; set; }

        public string OrderCreatedQueue { get; set; }
    }
}
