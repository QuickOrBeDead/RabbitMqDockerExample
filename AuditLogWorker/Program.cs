using AuditLogWorker;
using AuditLogWorker.Infrastructure.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton(typeof(IMessageQueueConsumerService), typeof(RabbitMqMessageQueueConsumerService));
        services.AddSingleton<IAuditService, AuditLogService>();
    })
    .Build();

await host.RunAsync();
