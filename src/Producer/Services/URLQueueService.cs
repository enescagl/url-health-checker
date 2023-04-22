using System.Text;

using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;

using URLHealthChecker.Producer.Interfaces;

namespace URLHealthChecker.Producer.Services
{
    public class URLQueueService : IQueueService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _model;

        public URLQueueService(IConfiguration configuration, IRabbitConnection rabbitConnection)
        {
            _configuration = configuration;
            _rabbitConnection = rabbitConnection.CreateConnection();
            _model = _rabbitConnection.CreateModel();
            _ = _model.QueueDeclare(queue: configuration["Queue:Name"],
                durable: Convert.ToBoolean(_configuration["Queue:Durable"]),
                exclusive: Convert.ToBoolean(_configuration["Queue:Exclusive"]),
                autoDelete: Convert.ToBoolean(_configuration["Queue:AutoDelete"]),
                arguments: null);
        }

        private static List<string> GetURLsFromFile()
        {
            List<string> urls = new();
            foreach (string urlFile in Directory.GetFiles("Resources", "*.txt"))
            {
                string? line;
                using StreamReader sr = new(urlFile);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        urls.Add(line);
                    }
                }
            }

            return urls;
        }

        public void SendURL()
        {
            List<string> urls = GetURLsFromFile();
            ConnectionFactory factory = new()
            {
                HostName = _configuration.GetValue<string>("RabbitMQ:HostName"),
                Port = _configuration.GetValue<int>("RabbitMQ:Port"),
                UserName = _configuration.GetValue<string>("RabbitMQ:UserName"),
                Password = _configuration.GetValue<string>("RabbitMQ:Password")
            };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            QueueDeclareOk queue = channel.QueueDeclare(queue: "urls",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            foreach (string url in urls)
            {
                string message = url;
                byte[] body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "urls",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}