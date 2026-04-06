using Confluent.Kafka;

public class KafkaProducerService
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService(ProducerConfig config)
    {
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        var kafkaMessage = new Message<Null, string>
        {
            Value = message
        };

        await _producer.ProduceAsync(topic, kafkaMessage);
    }


}