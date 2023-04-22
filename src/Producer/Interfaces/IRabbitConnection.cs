using RabbitMQ.Client;

namespace URLHealthChecker.Producer.Interfaces
{
    public interface IRabbitConnection
    {
        IConnection CreateConnection();
    }
}