using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Blogging;
using Northwind.Services.EntityFrameworkCore.Blogging.Context;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;

namespace Northwind.Services.EntityFrameworkCore.Blogging
{
    /// <summary>
    /// Provides a blog article products management service.
    /// </summary>
    public class BlogArticleProductManagementService : IBlogArticleProductsManagementService
    {
        private readonly BloggingContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogArticleProductManagementService"/> class.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public BlogArticleProductManagementService(string[] args)
        {
            this.context = new DesignTimeBloggingContextFactory().CreateDbContext(args);
        }

        /// <inheritdoc/>
        public async Task<IList<Models.BlogArticleProduct>> ShowBlogArticleProductsAsync(int articleId, int offset, int limit) =>
            limit == -1
                ? this.context.BlogArticleProducts.Where(articleProduct => articleProduct.ArticleId == articleId)
                    .Skip(offset)
                    .AsEnumerable()
                    .Select(GetBlogArticleProductModel)
                    .ToList()
                : this.context.BlogArticleProducts.Where(articleProduct => articleProduct.ArticleId == articleId)
                    .Skip(offset)
                    .Take(limit)
                    .AsEnumerable()
                    .Select(GetBlogArticleProductModel)
                    .ToList();

        /// <inheritdoc/>
        public async Task<bool> CreateBlogArticleProductLink(int articleId, int productId)
        {
            var articleProduct = new BlogArticleProduct()
            {
                ArticleId = articleId,
                ProductId = productId,
            };

            var entity = await this.context.AddAsync(articleProduct).ConfigureAwait(false);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveBlogArticleProductLink(int articleId, int productId)
        {
            var product = this.context.BlogArticleProducts.Where(articleProduct => articleProduct.ArticleId == articleId)
                                                          .FirstOrDefault(articleProduct => articleProduct.ProductId == productId);

            if (product is null)
            {
                return false;
            }

            this.context.Remove(product);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        private static BlogArticleProduct GetBlogArticleProductEntity(Models.BlogArticleProduct articleProduct) =>
            new ()
            {
                Id = articleProduct.Id,
                ArticleId = articleProduct.Id,
                ProductId = articleProduct.ProductId,
            };

        private static Models.BlogArticleProduct GetBlogArticleProductModel(BlogArticleProduct articleProduct) =>
            new ()
            {
                Id = articleProduct.Id,
                ArticleId = articleProduct.Id,
                ProductId = articleProduct.ProductId,
            };
    }
}
