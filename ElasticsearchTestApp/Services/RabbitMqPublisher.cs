//using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using ElasticsearchTestApp.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqPublisher : BackgroundService
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly ElasticsearchClient _elasticsearch;

    public RabbitMqPublisher(ElasticsearchClient elasticsearch)
    {
        _elasticsearch = elasticsearch;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "rabbituser",
            Password = "rabbitpassword"
        };

        _connection = await factory.CreateConnectionAsync(); // ✅ await
        _channel = await _connection.CreateChannelAsync();   // ✅ async API

        await _channel.QueueDeclareAsync(
            queue: "demo-queue",
            durable: false,

            exclusive: false,

            autoDelete: false,

            arguments: null
        );

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var article = JsonSerializer.Deserialize<ArticleDocument>(message);

                if (article != null)
                {
                    await IndexToElasticsearchAsync(article);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false); // ✅ await
            }
            catch
            {
                // можно сделать retry / DLQ
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "demo-queue",
            autoAck: false,
            consumer: consumer
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task IndexToElasticsearchAsync(ArticleDocument article)
    {
        var response = await _elasticsearch.IndexAsync(article);
        // Обработка ошибок и повторные попытки при необходимости
    }
}