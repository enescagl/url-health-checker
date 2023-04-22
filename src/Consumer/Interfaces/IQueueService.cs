namespace URLHealthChecker.Consumer.Interfaces
{
    public interface IQueueService
    {
        Queue<string> URLs { get; set; }
        Task ReceiveURLAsMessage();
    }
}