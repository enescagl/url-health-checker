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
            PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(30));
            while (await periodicTimer.WaitForNextTickAsync(stoppingToken))
            {
                await _queueService.ReceiveURLAsMessage();
                await _healthChecker.LogURLStatus(_queueService.URLs.Dequeue());
            }
        }
    }
}