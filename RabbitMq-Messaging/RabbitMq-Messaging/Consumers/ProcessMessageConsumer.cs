
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMq_Messaging.Interface;
using RabbitMq_Messaging.Models;
using RabbitMq_Messaging.Options;
using System.Text;

namespace RabbitMq_Messaging.Consumers
{
    public class ProcessMessageConsumer : BackgroundService
    {
        private readonly RabbitMqConfiguration _rabbitConfig;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public ProcessMessageConsumer(IOptions<RabbitMqConfiguration> option, IServiceProvider serviceProvider)
        {
            _rabbitConfig = option.Value;
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory { HostName = _rabbitConfig.Host };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _rabbitConfig.Queue,
                durable: false,
                autoDelete: false,
                exclusive: false,
                arguments: null);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var message = JsonConvert.DeserializeObject<InputMessageModel>(contentString);

                NotifyUser(message);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(_rabbitConfig.Queue, false, consumer);

            return Task.CompletedTask;
        }

        public void NotifyUser(InputMessageModel message)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var notifyUserService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            }
        }
    }
}
