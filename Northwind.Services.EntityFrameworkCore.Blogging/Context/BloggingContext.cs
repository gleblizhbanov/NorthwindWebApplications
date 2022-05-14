#pragma warning disable SA1600 // Elements should be documented.

using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Context
{
    public partial class BloggingContext : DbContext
    {

        public BloggingContext()
        {
        }

        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BlogArticle> BlogArticles { get; set; }

        public virtual DbSet<BlogArticleProduct> BlogArticleProducts { get; set; }

        public virtual DbSet<BlogComment> BlogComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BlogArticle>(entity =>
            {
                entity.HasKey(article => article.ArticleId).HasName("PK_Blog_Article");
            });

            modelBuilder.Entity<BlogArticleProduct>(entity =>
            {
                entity.HasKey(articleProduct => articleProduct.Id).HasName("PK_Blog_Article_Product");
            });

            modelBuilder.Entity<BlogComment>(entity =>
            {
                entity.HasKey(comment => comment.Id).HasName("PK_Blog_Comment");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
