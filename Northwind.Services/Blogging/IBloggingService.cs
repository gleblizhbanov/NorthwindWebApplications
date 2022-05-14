using System.Collections.Generic;
using System.Threading.Tasks;
using Northwind.Services.Models;

namespace Northwind.Services.Blogging
{
    /// <summary>
    /// Represents a blogging service.
    /// </summary>
    public interface IBloggingService
    {
        /// <summary>
        /// Creates a new article.
        /// </summary>
        /// <param name="article">An article to add.</param>
        /// <returns>A created article identifier.</returns>
        Task<int> CreateBlogArticleAsync(BlogArticle article);

        /// <summary>
        /// Deletes a blog article with specific identifier.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <returns>True if the article was deleted, false otherwise.</returns>
        Task<bool> DeleteBlogArticleAsync(int articleId);

        /// <summary>
        /// Shows a list of blog articles using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="BlogArticle"/>.</returns>
        Task<IList<BlogArticle>> ShowBlogArticlesAsync(int offset, int limit);

        /// <summary>
        /// Try to show a <see cref="BlogArticle"/> with specified identifier.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="article">A blog article to return.</param>
        /// <returns>Returns true if a blog article is returned; otherwise false.</returns>
        bool TryShowBlogArticle(int articleId, out BlogArticle article);

        /// <summary>
        /// Updates a blog article.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="article">A blog article.</param>
        /// <returns>True if an employee is updated; otherwise false.</returns>
        Task<bool> UpdateBlogArticleAsync(int articleId, BlogArticle article);
    }
}
