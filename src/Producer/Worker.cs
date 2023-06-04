using Microsoft.Extensions.Hosting;

using URLHealthChecker.Producer.Interfaces;

namespace URLHealthChecker.Producer
{
    public class Worker : BackgroundService
    {
        private readonly IQueueService _queueService;

        public Worker(IQueueService queueService)
        {
            _queueService = queueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(5));
            while (await periodicTimer.WaitForNextTickAsync(stoppingToken))
            {
                _queueService.SendURL();
            }
        }
    }
}