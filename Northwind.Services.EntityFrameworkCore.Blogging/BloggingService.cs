using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Blogging;
using Northwind.Services.EntityFrameworkCore.Blogging.Context;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;

namespace Northwind.Services.EntityFrameworkCore.Blogging
{
    /// <summary>
    /// Provides a blogging service.
    /// </summary>
    public class BloggingService : IBloggingService
    {
        private readonly BloggingContext bloggingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BloggingService"/> class.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public BloggingService(string[] args)
        {
            this.bloggingContext = new DesignTimeBloggingContextFactory().CreateDbContext(args);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloggingService"/> class.
        /// </summary>
        /// <param name="bloggingContext">A blogging database context.</param>
        public BloggingService(BloggingContext bloggingContext)
        {
            this.bloggingContext = bloggingContext;
        }

        /// <inheritdoc/>
        public async Task<int> CreateBlogArticleAsync(Models.BlogArticle article)
        {
            var entity = await this.bloggingContext.AddAsync(GetArticleEntity(article)).ConfigureAwait(false);
            await this.bloggingContext.SaveChangesAsync().ConfigureAwait(false);
            return entity.Entity.ArticleId;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteBlogArticleAsync(int articleId)
        {
            var entity = await this.bloggingContext.FindAsync<BlogArticle>(articleId).ConfigureAwait(false);
            if (entity is null)
            {
                return false;
            }

            this.bloggingContext.Remove(entity);
            return await this.bloggingContext.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public async Task<IList<Models.BlogArticle>> ShowBlogArticlesAsync(int offset, int limit)
        {
            return limit != int.MaxValue
                ? this.bloggingContext.BlogArticles.Skip(offset).Take(limit).AsEnumerable().Select(GetArticleModel).ToList()
                : this.bloggingContext.BlogArticles.Skip(offset).AsEnumerable().Select(GetArticleModel).ToList();
        }

        /// <inheritdoc/>
        public bool TryShowBlogArticle(int articleId, out Models.BlogArticle article)
        {
            var articleEntity = this.bloggingContext.Find<BlogArticle>(articleId);
            if (articleEntity is null)
            {
                article = null;
                return false;
            }

            article = GetArticleModel(articleEntity);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateBlogArticleAsync(int articleId, Models.BlogArticle article)
        {
            var articleEntity = await this.bloggingContext.FindAsync<BlogArticle>(articleId).ConfigureAwait(false);
            if (articleEntity is null)
            {
                return false;
            }

            articleEntity.Title = article.Title;
            articleEntity.Text = article.Text;
            this.bloggingContext.Update(articleEntity);
            return await this.bloggingContext.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        private static Models.BlogArticle GetArticleModel(BlogArticle article) =>
            new ()
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Text = article.Text,
                Posted = article.Posted,
                AuthorId = article.AuthorId,
            };

        private static BlogArticle GetArticleEntity(Models.BlogArticle article) =>
            new ()
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Text = article.Text,
                Posted = article.Posted,
                AuthorId = article.AuthorId,
            };
    }
}
