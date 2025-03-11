using Confluent.Kafka;
using System.Text.Json;

namespace UserProfileService.Services
{
    public class KafkaProducer
    {
        private readonly string _bootstrapServers;

        public KafkaProducer(string bootstrapServers)
        {
            _bootstrapServers = bootstrapServers;
        }

        public async Task PublishUserCredentialsAsync(string topic, object message)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var jsonMessage = JsonSerializer.Serialize(message);

            try
            {
                var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });
                Console.WriteLine($"Message sent to {result.TopicPartitionOffset}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing message: {ex.Message}");
            }
        }
    }
}
