using Confluent.Kafka;
using ElasticsearchTestApp.Models;
using System.Text.Json;

public class ArticleKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public ArticleKafkaProducer(ProducerConfig config)
    {
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(ArticleDocument article)
    {
        var json = JsonSerializer.Serialize(article, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await _producer.ProduceAsync("articles", new Message<string, string>
        {
            Key = article.Id.ToString(),
            Value = json
        });
    }
}