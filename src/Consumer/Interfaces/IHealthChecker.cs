namespace URLHealthChecker.Consumer.Interfaces
{
    public interface IHealthChecker
    {
        Task LogURLStatus(string url);
    }
}