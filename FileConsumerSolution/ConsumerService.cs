using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FileConsumerSolution
{
    public class ConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel;
        private const string QueueName = "fileProcessingQueue";
        private int _maxRequestsPerSecond;
        private TimeSpan _rateLimitDelay;

        public ConsumerService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;

            _maxRequestsPerSecond = _configuration.GetValue<int>("RateLimit:MaxRequestsPerSecond", 30);
            _rateLimitDelay = TimeSpan.FromMilliseconds(1000 / _maxRequestsPerSecond);

            InitializeRabbitMQAsync().Wait();
        }

        private async Task InitializeRabbitMQAsync()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", ConsumerDispatchConcurrency = 1 };
            _connection = await factory.CreateConnectionAsync();
            _channel  = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) => await ProcessMessageAsync(ea, stoppingToken);

            await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
            await Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                try
                {
                    context.FileRecords.Add(new FileData { Content = message });
                    await context.SaveChangesAsync(stoppingToken);

                    // Acknowledge the message
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true); // Requeue the message
                }
            }

            await Task.Delay(_rateLimitDelay, stoppingToken); // Apply rate limiting when rate limit is exceeded
        }

        public override void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            base.Dispose();
        }
    }
}