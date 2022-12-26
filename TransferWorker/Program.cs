using Microsoft.EntityFrameworkCore;

using TransferWorker;
using TransferWorker.Infrastructure;
using TransferWorker.Infrastructure.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton(typeof(IMessageQueueConsumerService<>), typeof(RabbitMqMessageQueueConsumerService<>));
        services.AddScoped<IBankService, BankService>();

        services.AddDbContext<TransferDbContext>(options =>
            options.UseNpgsql("Host=postgres;Database=transferdb;Username=postgres;Password=postgres"));
    })
    .Build();

using (var serviceScope = host.Services
           .GetRequiredService<IServiceScopeFactory>()
           .CreateScope())
{
    using (var context = serviceScope.ServiceProvider.GetRequiredService<TransferDbContext>())
    {
        context.Database.EnsureCreated();
    }
}

await host.RunAsync().ConfigureAwait(false);
