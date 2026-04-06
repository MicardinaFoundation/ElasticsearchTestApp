using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using ElasticsearchTestApp.Models;
using System.Text.Json;

public class KafkaToElasticHostedService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ElasticsearchClient _client;

    public KafkaToElasticHostedService(
        IConsumer<string, string> consumer,
        ElasticsearchClient client)
    {
        _consumer = consumer;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("articles");

        // Даем хосту завершить старт
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);

            if (result?.Message == null)
                continue;

            var document = JsonSerializer.Deserialize<ArticleDocument>(result.Message.Value);

            if (document == null)
                continue;

            // Upsert по Id
            await _client.IndexAsync(document, i => i
                .Id(document.Id)
                .Index("articles")
            );

            // Коммит offset ТОЛЬКО после успешной записи
            _consumer.Commit(result);
        }
    }
}