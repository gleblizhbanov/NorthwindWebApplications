using System.Collections.Generic;
using System.Threading.Tasks;
using Northwind.Services.Models;

namespace Northwind.Services.Blogging
{
    /// <summary>
    /// Represents a blog comments managements service.
    /// </summary>
    public interface IBlogCommentsManagementService
    {
        /// <summary>
        /// Creates a new blog comment.
        /// </summary>
        /// <param name="comment">A blog comment to add.</param>
        /// <returns>A blog comment identifier.</returns>
        Task<int> CreateBlogCommentAsync(BlogComment comment);

        /// <summary>
        /// Shows a list of blog comments using specified offset and limit for pagination.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="BlogComment"/>.</returns>
        Task<IList<BlogComment>> ShowBlogCommentsAsync(int articleId, int offset, int limit);

        /// <summary>
        /// Tries to show a blog comment with specific identifier.
        /// </summary>
        /// <param name="commentId">A blog comment identifier.</param>
        /// <param name="comment">A comment to return.</param>
        /// <returns>True if the comment was returned, false otherwise.</returns>
        bool TryShowBlogComment(int commentId, out BlogComment comment);

        /// <summary>
        /// Updates a blog comment with specific identifier.
        /// </summary>
        /// <param name="commentId">A blog comment identifier.</param>
        /// <param name="comment">A blog comment to replace old comment with.</param>
        /// <returns>True if the comment was updated, false otherwise.</returns>
        Task<bool> UpdateBlogCommentAsync(int commentId, BlogComment comment);

        /// <summary>
        /// Deletes a blog comment with specific identifier.
        /// </summary>
        /// <param name="commentId">A blog comment identifier.</param>
        /// <returns>True if the comment was deleted, false otherwise.</returns>
        Task<bool> DeleteBlogCommentAsync(int commentId);
    }
}
