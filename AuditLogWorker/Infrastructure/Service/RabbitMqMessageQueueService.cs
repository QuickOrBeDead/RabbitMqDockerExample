namespace AuditLogWorker.Infrastructure.Service
{
    using System.Text;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public interface IMessageQueueConsumerService : IDisposable
    {
        void ConsumeMessage(string queueName, Action<string> consumeAction);
    }

    public class RabbitMqMessageQueueConsumerService : IMessageQueueConsumerService
    {
        private bool _disposed;

        private IConnection? _connection;

        private IModel? _channel;

        public void ConsumeMessage(string queueName, Action<string> consumeAction)
        {
            if (consumeAction == null)
            {
                throw new ArgumentNullException(nameof(consumeAction));
            }

            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(queueName));
            }

            // TODO: reconnect ??
            var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("DockerExample", "fanout", true, false, null);
            _channel.QueueDeclare(queueName, false, false, false, null);
            _channel.QueueBind(queueName, "DockerExample", string.Empty);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, e) =>
                {
                    consumeAction(Encoding.UTF8.GetString(e.Body.Span));
                };
            _channel.BasicConsume(queueName, true, consumer);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                _channel?.Dispose();
                _connection?.Dispose();
                _disposed = true;
            }
        }

        ~RabbitMqMessageQueueConsumerService()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
