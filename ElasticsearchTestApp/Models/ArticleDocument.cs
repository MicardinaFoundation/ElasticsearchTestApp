namespace ElasticsearchTestApp.Models
{
    public class ArticleDocument
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        
        public string Content { get; set; } = string.Empty;
    }
}
