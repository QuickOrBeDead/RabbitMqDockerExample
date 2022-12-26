namespace AuditLogWorker
{
    using AuditLogWorker.Infrastructure.Service;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAuditService _auditService;
        private readonly IMessageQueueConsumerService _messageQueueConsumerService;

        public Worker(
            IAuditService auditLogService,
            IMessageQueueConsumerService messageQueueConsumerService,
            ILogger<Worker> logger)
        {
            _auditService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageQueueConsumerService = messageQueueConsumerService ?? throw new ArgumentNullException(nameof(messageQueueConsumerService));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("AuditLog consumer starting at: {time}", DateTime.Now);
                _messageQueueConsumerService.ConsumeMessage(
                    "Auditlog",
                    message =>
                        {
                            _auditService.Log(message);
                        });
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AuditLog consumer stopping at: {time}", DateTime.Now);
            _messageQueueConsumerService.Dispose();

            return Task.CompletedTask;
        }
    }
}