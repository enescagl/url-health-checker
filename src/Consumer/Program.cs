using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Filters;

using URLHealthChecker.Consumer;
using URLHealthChecker.Consumer.Interfaces;
using URLHealthChecker.Consumer.Services;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((services) =>
    {
        _ = services.AddHttpClient();
        _ = services.AddTransient<IHealthChecker, HealthChecker>();
        _ = services.AddSingleton<IRabbitConnection, RabbitConnection>();
        _ = services.AddTransient<IQueueService, URLQueueService>();
        _ = services.AddHostedService<Worker>();
    })
    .UseSerilog((context, _, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(Matching.FromSource("URLHealthChecker.Consumer.Services"))
            .WriteTo.Console(Serilog.Events.LogEventLevel.Information, "{Message}\n"))
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(Matching.FromSource("Microsoft.Hosting.Lifetime"))
            .WriteTo.Console(Serilog.Events.LogEventLevel.Information))
    )
    .Build();

host.Run();