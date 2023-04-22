
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using URLHealthChecker.Producer.Interfaces;
using URLHealthChecker.Producer.Services;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((services) => _ = services.AddTransient<IQueueService, URLQueueService>())
    // .ConfigureLogging((_, logging) => logging.Services.AddLogging())
    .Build();

host.Services.GetRequiredService<IQueueService>().SendURL();
