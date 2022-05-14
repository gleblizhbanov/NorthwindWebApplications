using System.Collections.Generic;
using System.Threading.Tasks;
using Northwind.Services.Models;

namespace Northwind.Services.Blogging
{
    /// <summary>
    /// Represents a management service for blog article products.
    /// </summary>
    public interface IBlogArticleProductsManagementService
    {
        /// <summary>
        /// Shows a list of blog article products using specified offset and limit for pagination.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="BlogArticleProduct"/>.</returns>
        Task<IList<BlogArticleProduct>> ShowBlogArticleProductsAsync(int articleId, int offset, int limit);

        /// <summary>
        /// Create a new link to a product from an article.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="productId">A product identifier.</param>
        /// <returns>True if the article with given identifier exists and the it doesn't have a link to a product with given identifier, false otherwise.</returns>
        Task<bool> CreateBlogArticleProductLink(int articleId, int productId);

        /// <summary>
        /// Removes an existing link to a product from an article.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="productId">A product identifier.</param>
        /// <returns>True if the article with given identifier exists and the link to a product with given identifier existed, false otherwise.</returns>
        Task<bool> RemoveBlogArticleProductLink(int articleId, int productId);
    }
}
