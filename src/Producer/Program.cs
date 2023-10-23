using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Filters;

using URLHealthChecker.Producer;
using URLHealthChecker.Producer.Interfaces;
using URLHealthChecker.Producer.Services;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((services) =>
    {
        _ = services.AddTransient<IRabbitConnection, RabbitConnection>();
        _ = services.AddTransient<IFileReader, URLFileReader>();
        _ = services.AddTransient<IQueueService, URLQueueService>();
        _ = services.AddHostedService<Worker>();
    })
    .UseSerilog((context, _, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Logger(lc => lc
            .Filter.ByExcluding(Matching.FromSource("Microsoft.Hosting.Lifetime"))
            .WriteTo.Console(Serilog.Events.LogEventLevel.Information))
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(Matching.FromSource("Microsoft.Hosting.Lifetime"))
            .WriteTo.Console(Serilog.Events.LogEventLevel.Information))
    ).Build();

host.Run();
