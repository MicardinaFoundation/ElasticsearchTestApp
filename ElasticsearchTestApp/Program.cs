
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

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
