using Microsoft.Extensions.Hosting;

using URLHealthChecker.Consumer.Interfaces;

namespace URLHealthChecker.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly IQueueService _queueService;
        private readonly IHealthChecker _healthChecker;

        public Worker(IQueueService queueService, IHealthChecker healthChecker)
        {
            _queueService = queueService;
            _healthChecker = healthChecker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _queueService.ReceiveURLAsMessage();
                if (_queueService.URLs.TryDequeue(out string? urlOutQueue))
                {
                    await _healthChecker.LogURLStatus(urlOutQueue);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}