using Elastic.Clients.Elasticsearch;
using ElasticsearchTestApp.Data;
using ElasticsearchTestApp.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/search")]
public class ArticlesController : ControllerBase
{
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ElasticsearchClient _elasticClient;
    //private readonly string _elasticIndex;
    private readonly ArticleDbContext _context;

    private readonly RabbitMqProducerService _rabbitMqProducer;

    public ArticlesController(ElasticsearchClient service, IKafkaProducerService kafkaProducer, RabbitMqProducerService rabbitMqProducer, IConfiguration configuration, ArticleDbContext articleDbContext)
    {
        _elasticClient = service;
        _kafkaProducer = kafkaProducer;
        _rabbitMqProducer = rabbitMqProducer;
        _context = articleDbContext;
    }


    /// <summary>
    /// Публикация документа в кафке
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Результат</returns>
    [HttpPost("index")]
    public async Task<IActionResult> CreateArticle([FromBody] ArticleDocument request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Id.ToString()))
        {
            return BadRequest("Некорректный документ или отсутствует Id.");
        }

        //await _rabbitMqProducer.SendAsync(request);

        //// Публикуем в Kafka. Приложение больше ничего не делает.
        //await _kafkaProducer.ProduceAsync(request);

        await _context.ArticleDocuments.AddAsync(request);
        await _context.SaveChangesAsync();

        return Accepted(new { Message = "Статья отправлена в Apache Kafka. Индексация будет выполнена автоматически через Kafka Connect." });
    }

    /// <summary>
    /// Осуществляет поиск напрямую из Elastic
    /// </summary>
    /// <param name="query">Строка поиска</param>
    /// <returns>Массив результатов</returns>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var response = await _elasticClient.SearchAsync<ArticleDocument>(s => s
            .Indices("articles")
            .Query(q => q
                //.Match(m => m
                //    .Field(f => f.Content)
                //    .Query(query)
                //)
                //.MultiMatch(m => m
                //    //.Fields(f => f.MultiField(f.Title))
                //    //.Fields(f => f.MultiField(f.Content))
                //    .Fields(f => f.)
                //    .Query(query)
                //    .Type(TextQueryType.BestFields)
                //)
                .Bool(b => b
                .Should(
                    sh => sh.Match(m => m
                        .Field(f => f.Title)
                        .Query(query)
                        .Boost(2) // важнее
                    ),
                    sh => sh.Match(m => m
                        .Field(f => f.Content)
                        .Query(query)
                    )
                )
            )
            )
        //.Query(q => q
        //.Bool(b => b
        //    .Should(
        //        qm => qm.Match(m => m.Field("title").Query(query)),
        //        qm => qm.Match(m => m.Field("content").Query(query))
        //    )
        //)

        //)
        //.Highlight(h => h.Fields(
        //    f => f.F("title"),
        //    f => f.Field("content")
        //))
        );


        return Ok(response.Documents);
    }
}