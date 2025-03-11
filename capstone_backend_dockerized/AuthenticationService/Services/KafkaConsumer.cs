using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AuthenticationService.Data;
using AuthenticationService.Models;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Services
{
    public class KafkaConsumerService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly string _topic = "user-credentials";
        private readonly string _bootstrapServers = "kafka:9092";
        private readonly string _groupId = "auth-service-group";
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts;

        public KafkaConsumerService(IServiceScopeFactory scopeFactory, ILogger<KafkaConsumerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _stoppingCts = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer Service is starting.");
            _executingTask = Task.Run(() => ExecuteAsync(_stoppingCts.Token), cancellationToken);
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        private async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        var message = consumeResult.Message.Value;

                        _logger.LogInformation($"Received message: {message}");

                        var user = JsonSerializer.Deserialize<UserCredential>(message);
                        if (user != null)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<AuthContext>();

                            dbContext.UserCredentials.Add(user);
                            await dbContext.SaveChangesAsync(stoppingToken);

                            _logger.LogInformation("User saved successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error consuming message: {ex.Message}");
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer Service is stopping.");
            _stoppingCts.Cancel();
            await _executingTask;
        }
    }
}