using ElasticsearchTestApp.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ArticleSearchService _service;

    private readonly ArticleKafkaProducer _kafkaProducer;

    private readonly RabbitMqProducerService _rabbitMqProducer;

    public SearchController(ArticleSearchService service, ArticleKafkaProducer kafkaProducer, RabbitMqProducerService rabbitMqProducer)
    {
        _service = service;
        _kafkaProducer = kafkaProducer;
        _rabbitMqProducer = rabbitMqProducer;
    }



    [HttpPost("index")]
    //public async Task<IActionResult> Index([FromBody] ArticleDocument article)
    //{
    //    //await _kafkaProducer.PublishAsync(article);
    //    await _rabbitMqProducer.SendAsync(article);
    //    //var documents = new[]
    //    //{
    //    //    new ArticleDocument { Id = 1, Title = "Depelovler", Content = "Моя первая статья по ASP.NET Core и Elasticsearch" },
    //    //    new ArticleDocument { Id = 2, Title = "Voloder", Content = "Полнотекстовый поиск в .NET" },
    //    //    new ArticleDocument { Id = 3, Title = "Gorin", Content = "Работа с Elasticsearch 9 - быстрый старт" },
    //    //    new ArticleDocument { Id = 4, Title = "Voloder", Content = "Работа с Elasticsearch 10 - быстрый старт" }
    //    //};

    //    //await _service.IndexAsync(documents);



    //    return Ok();
    //}
    public async Task<IActionResult> CreateArticle([FromBody] ArticleDocument request)
    {
        //var article = new ArticleDocument
        //{
        //    Title = request.Title,
        //    Content = request.Content
        //};

        await _rabbitMqProducer.SendAsync(request);

        return Accepted(); // HTTP 202 Accepted
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var result = await _service.SearchAsync(q);
        return Ok(result);
    }
}