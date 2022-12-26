namespace PresentationApi.Infrastructure.Service
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Text.Json;

    using RabbitMQ.Client;

    public interface IMessageQueueService
    {
        void PublishMessage<TModel>(string queueName, [DisallowNull] TModel model);
    }

    public class RabbitMqMessageQueueService : IMessageQueueService
    {
        public void PublishMessage<TModel>(string queueName, [DisallowNull] TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(queueName));
            }

            var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("DockerExample", "fanout", true, false, null);
                channel.QueueDeclare(queueName, false, false, false, null);
                channel.BasicPublish(
                    "DockerExample",
                    string.Empty,
                    null,
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model)));
            }
        }
    }
}
