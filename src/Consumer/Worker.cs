using Microsoft.Extensions.Hosting;

using URLHealthChecker.Consumer.Interfaces;

namespace URLHealthChecker.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly IHealthChecker _healthChecker;
        private readonly IQueueService _queueService;

        public Worker(IHealthChecker healthChecker, IQueueService queueService)
        {
            _healthChecker = healthChecker;
            _queueService = queueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Worker running at: {DateTimeOffset.Now}");
                await _queueService.ReceiveURLAsMessage();
                if (_queueService.URLs.Count > 0)
                {
                    string url = _queueService.URLs.Dequeue();
                    await _healthChecker.LogURLStatus(url);
                }
            }
        }
    }
}