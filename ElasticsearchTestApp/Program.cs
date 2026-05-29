
using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using RabbitMQ.Client;

namespace ElasticsearchTestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
                .Authentication(new BasicAuthentication("elastic", "elastic_password"))
                .DefaultIndex("articles");

            // Клиент регистрируется как Singleton
            builder.Services.AddSingleton(new ElasticsearchClient(settings));


            #region - Kafka produced -

            builder.Services.AddSingleton(new ProducerConfig
            {
                BootstrapServers = "localhost:9094"
            });

            builder.Services.AddSingleton<KafkaProducerService>();
            builder.Services.AddSingleton<ArticleKafkaProducer>();

            #endregion

            #region - Kafka consumer -

            //builder.Services.AddSingleton<IConsumer<string, string>>(_ =>
            //{
            //    var config = new ConsumerConfig
            //    {
            //        BootstrapServers = "localhost:9094",
            //        GroupId = "article-indexer",
            //        AutoOffsetReset = AutoOffsetReset.Earliest,
            //        EnableAutoCommit = false
            //    };

            //    return new ConsumerBuilder<string, string>(config)
            //    .Build();
            //});

            //builder.Services.AddHostedService<KafkaToElasticHostedService>();

            #endregion


            #region - RabbitMQ -

            builder.Services.AddSingleton(new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "rabbituser",
                Password = "rabbitpassword"
            });

            builder.Services.AddSingleton<RabbitMqProducerService>();

            #endregion

            #region - RabbitMQ Consumer -

            builder.Services.AddHostedService<RabbitMqPublisher>();
            #endregion




            builder.Services.AddScoped<ArticleSearchService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
