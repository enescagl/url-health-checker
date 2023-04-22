using RabbitMQ.Client;

namespace URLHealthChecker.Consumer.Interfaces
{
    public interface IRabbitConnection
    {
        IConnection CreateConnection();
    }
}