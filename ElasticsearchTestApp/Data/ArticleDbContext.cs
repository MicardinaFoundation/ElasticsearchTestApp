using ElasticsearchTestApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ElasticsearchTestApp.Data
{
    public class ArticleDbContext : DbContext
    {
        public virtual DbSet<ArticleDocument> ArticleDocuments { get; set; }

        public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options)
        {
            base.Database.SetCommandTimeout(30);
        }
    }
}
