using System.Text.Json;
using Confluent.Kafka;
using ElasticsearchTestApp.Models;

public class KafkaProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topicName;

    public KafkaProducerService(ProducerConfig config)
    {
        _topicName = configuration["Kafka:TopicName"] ?? "articles-topic";

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(ArticleDocument article)
    {
        var jsonValue = JsonSerializer.Serialize(article);
        var kafkaMessage = new Message<Null, string>
        {
            Key = article.Id,
            Value = jsonValue
        };

        await _producer.ProduceAsync(topic, kafkaMessage);
    }


}