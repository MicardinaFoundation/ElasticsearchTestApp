using System.Text.Json;
using Confluent.Kafka;
using ElasticsearchTestApp.Models;
public interface IKafkaProducerService
{
    Task ProduceAsync(ArticleDocument article);
}
public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<int, string> _producer;
    private readonly string _topicName;

    public KafkaProducerService(IConfiguration configuration)
    {
        _topicName = configuration["Kafka:TopicName"] ?? "articles";

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            // Гарантия доставки сообщения на брокер
            Acks = Acks.All
        };

        _producer = new ProducerBuilder<int, string>(config).Build();
    }

    public async Task ProduceAsync(ArticleDocument article)
    {
        var jsonValue = JsonSerializer.Serialize(article, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var kafkaMessage = new Message<int, string>
        {
            Key = article.Id,
            Value = jsonValue
        };

        await _producer.ProduceAsync(_topicName, kafkaMessage);
    }


}