using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;

using URLHealthChecker.Producer.Interfaces;

namespace URLHealthChecker.Producer.Services
{
    public class RabbitConnection : IRabbitConnection
    {
        private readonly IConfiguration _configuration;

        public RabbitConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConnection CreateConnection()
        {
            ConnectionFactory factory = new()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                DispatchConsumersAsync = true
            };
            return factory.CreateConnection();
        }
    }
}