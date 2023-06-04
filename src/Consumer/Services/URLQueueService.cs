using System.Collections.Concurrent;
using System.Text;

using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using URLHealthChecker.Consumer.Interfaces;

namespace URLHealthChecker.Consumer.Services
{
    public class URLQueueService : IQueueService, IDisposable
    {
        public ConcurrentQueue<string> URLs { get; set; } = new();

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
        public async Task ReceiveURLAsMessage()
        {
            AsyncEventingBasicConsumer consumer = new(_model);

            consumer.Received += QueueMessages;

            _ = _model.BasicConsume(queue: _configuration["Queue:Name"],
                autoAck: true,
                consumer: consumer);

            await Task.CompletedTask;
        }

        private async Task QueueMessages(object? model, BasicDeliverEventArgs ea)
        {
            byte[] body = ea.Body.ToArray();
            string text = Encoding.UTF8.GetString(body);
            if (text != string.Empty)
            {
                _ = URLs.Append(text);
            }
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_model.IsOpen)
            {
                _model.Close();
            }
            if (_rabbitConnection.IsOpen)
            {
                _rabbitConnection.Close();
            }
        }
    }
}