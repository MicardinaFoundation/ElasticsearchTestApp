using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Esql.Extensions;
using ElasticsearchTestApp.Models;


public class ArticleSearchService
{
    private readonly ElasticsearchClient _client;

    public ArticleSearchService(ElasticsearchClient client)
    {
        _client = client;
    }


    public async Task IndexAsync(IEnumerable<ArticleDocument> documents)
    {
        // Bulk-индексация - предпочтительный способ записи
        var response = await _client.BulkAsync(b => b
            .Index("articles")
            .IndexMany(documents)
        );

        if (response.Errors)
        {
            throw new InvalidOperationException("Ошибка при индексации документов");
        }
    }

    public async Task<IReadOnlyCollection<ArticleDocument>> SearchAsync(string query)
    {
        var response = await _client.SearchAsync<ArticleDocument>(s => s
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

        return response.Documents;
    }

}