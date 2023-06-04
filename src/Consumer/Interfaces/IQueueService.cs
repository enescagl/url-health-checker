using System.Collections.Concurrent;

namespace URLHealthChecker.Consumer.Interfaces
{
    public interface IQueueService
    {
        ConcurrentQueue<string> URLs { get; set; }
        Task ReceiveURLAsMessage();
    }
}