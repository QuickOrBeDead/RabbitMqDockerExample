namespace TransferWorker
{
    using TransferWorker.Infrastructure.Model;
    using TransferWorker.Infrastructure.Service;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IServiceProvider _serviceProvider;

        private readonly IMessageQueueConsumerService<TransferModel> _messageQueueConsumerService;

        public Worker(
            IServiceProvider serviceProvider,
            IMessageQueueConsumerService<TransferModel> messageQueueConsumerService,
            ILogger<Worker> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageQueueConsumerService = messageQueueConsumerService ?? throw new ArgumentNullException(nameof(messageQueueConsumerService));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Transfer consumer starting at: {time}", DateTime.Now);
                _messageQueueConsumerService.ConsumeMessage(
                    "Transfer",
                    model =>
                        {
                            using (IServiceScope scope = _serviceProvider.CreateScope())
                            {
                                var bankService = scope.ServiceProvider.GetRequiredService<IBankService>();

                                bankService.Transfer(model);
                            }
                        });
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Transfer consumer stopping at: {time}", DateTime.Now);
            _messageQueueConsumerService.Dispose();

            return Task.CompletedTask;
        }
    }
}