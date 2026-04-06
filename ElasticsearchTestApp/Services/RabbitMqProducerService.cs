using ElasticsearchTestApp.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqProducerService
{
    private readonly ConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqProducerService(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        _connection ??= await _connectionFactory.CreateConnectionAsync();

        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(
            queue: "demo-queue",

            durable: false,

            exclusive: false,

            autoDelete: false,

            arguments: null
        );
    }

    public async Task SendAsync(ArticleDocument article)
    {
        if (_channel is null)
        {
            await InitializeAsync();
        }

        var message = JsonSerializer.Serialize(article);
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: "demo-queue",
            body: body
        );
    }
}