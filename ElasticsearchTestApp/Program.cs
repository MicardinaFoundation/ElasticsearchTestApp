
using System;
using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using ElasticsearchTestApp.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace ElasticsearchTestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Подключение MariaDB
            var connectionString = builder.Configuration.GetConnectionString("ArticleDbConnection");
            builder.Services.AddDbContext<ArticleDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
                .Authentication(new BasicAuthentication("elastic", "elastic_password"))
                .DefaultIndex("articles");

            // Клиент регистрируется как Singleton
            builder.Services.AddSingleton(new ElasticsearchClient(settings));

            // Регистрация Kafka Producer
            builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

            //#region - Kafka produced -

            //builder.Services.AddSingleton(new ProducerConfig
            //{
            //    BootstrapServers = "localhost:9094"
            //});

            //builder.Services.AddSingleton<KafkaProducerService>();
            //builder.Services.AddSingleton<ArticleKafkaProducer>();

            //#endregion



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





            var app = builder.Build();

            // Автоматическое создание таблицы в БД при старте приложения, если её нет
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();
                db.Database.EnsureCreated();
            }

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
