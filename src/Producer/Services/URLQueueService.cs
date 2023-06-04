using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

using URLHealthChecker.Producer.Interfaces;

namespace URLHealthChecker.Producer.Services
{
    public class URLQueueService : IQueueService
    {
        private readonly IConfiguration _configuration;
        private readonly IFileReader _fileReader;
        private readonly ILogger<URLQueueService> _logger;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _model;

        public URLQueueService(
            IConfiguration configuration,
            IRabbitConnection rabbitConnection,
            IFileReader fileReader,
            ILogger<URLQueueService> logger)
        {
            _configuration = configuration;
            _fileReader = fileReader;
            _logger = logger;
            _rabbitConnection = rabbitConnection.CreateConnection();
            _model = _rabbitConnection.CreateModel();
            _ = _model.QueueDeclare(
                queue: _configuration["Queue:Name"],
                durable: Convert.ToBoolean(_configuration["Queue:Durable"]),
                exclusive: Convert.ToBoolean(_configuration["Queue:Exclusive"]),
                autoDelete: Convert.ToBoolean(_configuration["Queue:AutoDelete"]),
                arguments: null);
        }

        public void SendURL()
        {
            List<string> urls = _fileReader.ReadLinesInFolder("Resources");
            _logger.LogInformation("Sending {Count} URLs to queue", urls.Count);
            ConnectionFactory factory = new()
            {
                HostName = _configuration.GetValue<string>("RabbitMQ:HostName"),
                Port = _configuration.GetValue<int>("RabbitMQ:Port"),
                UserName = _configuration.GetValue<string>("RabbitMQ:UserName"),
                Password = _configuration.GetValue<string>("RabbitMQ:Password")
            };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            _ = channel.QueueDeclare(queue: "urls",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            foreach (string url in urls)
            {
                byte[] body = Encoding.UTF8.GetBytes(url);

                channel.BasicPublish(exchange: "",
                                     routingKey: "urls",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}