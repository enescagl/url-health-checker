using Microsoft.Extensions.Hosting;

using URLHealthChecker.Producer.Interfaces;

namespace URLHealthChecker.Producer
{
    public class Worker : BackgroundService
    {
        private readonly IWatcher _watcher;

        public Worker(IWatcher watcher)
        {
            _watcher = watcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Watching files");
                await _watcher.Watch();
            }
        }
    }
}