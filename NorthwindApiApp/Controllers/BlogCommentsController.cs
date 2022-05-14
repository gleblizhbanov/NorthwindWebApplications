using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Blogging;
using Northwind.Services.Models;

namespace NorthwindApiApp.Controllers
{
    /// <summary>
    /// Provides a blog comments controller.
    /// </summary>
    [Route("api/articles")]
    [ApiController]
    public class BlogCommentsController : ControllerBase
    {
        private readonly IBloggingService bloggingService;
        private readonly IBlogCommentsManagementService commentsManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogCommentsController"/> class.
        /// </summary>
        /// <param name="commentsManagementService">A blog comments management service.</param>
        public BlogCommentsController(IBlogCommentsManagementService commentsManagementService, IBloggingService bloggingService)
        {
            this.commentsManagementService = commentsManagementService;
            this.bloggingService = bloggingService;
        }

        /// <summary>
        /// Creates a new blog comment.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="comment">A blog comment to post.</param>
        /// <returns>An action result.</returns>
        [HttpPost("{articleId:int}/comments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBlogCommentAsync(int articleId, BlogComment comment)
        {
            comment.ArticleId = articleId;

            if (!this.bloggingService.TryShowBlogArticle(comment.ArticleId, out _))
            {
                return this.NotFound();
            }

            if (await this.commentsManagementService.CreateBlogCommentAsync(comment).ConfigureAwait(false) > 0)
            {
                return this.CreatedAtAction("CreateBlogComment", new { id = comment.Id }, comment);
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Return all blog article comments.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <returns>A list of blog comments.</returns>
        [HttpGet("{articleId:int}/comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IList<BlogComment>>> GetBlogCommentsAsync(int articleId)
        {
            if (!this.bloggingService.TryShowBlogArticle(articleId, out _))
            {
                return this.NotFound();
            }

            if (await this.commentsManagementService.ShowBlogCommentsAsync(articleId, 0, int.MaxValue).ConfigureAwait(false) is not List<BlogComment> comments)
            {
                return this.BadRequest();
            }

            return comments;
        }

        /// <summary>
        /// Updates a blog comment with specific identifier.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="commentId">A blog comment identifier.</param>
        /// <param name="comment">A new comment.</param>
        /// <returns>An action result/</returns>
        [HttpPut("{articleId:int}/comments/{commentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBlogCommentAsync(int articleId, int commentId, BlogComment comment)
        {
            if (!this.bloggingService.TryShowBlogArticle(articleId, out _))
            {
                return this.NotFound();
            }

            if (await this.commentsManagementService.UpdateBlogCommentAsync(commentId, comment).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Deletes a blog comment with specific identifier.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="commentId">A blog commnet identifier.</param>
        /// <returns>An action result.</returns>
        [HttpDelete("{articleId:int}/comments/{commentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteBlogCommentAsync(int articleId, int commentId)
        {
            if (!this.bloggingService.TryShowBlogArticle(articleId, out _))
            {
                return this.NotFound();
            }

            if (await this.commentsManagementService.DeleteBlogCommentAsync(commentId).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Gets a blog article comment with specific identifier.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="commentId">A blog comment identifier.</param>
        /// <returns>A blog comment.</returns>
        [HttpGet("{articleId:int}/comments/{commentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BlogComment> GetBlogComment(int articleId, int commentId)
        {
            if (!this.bloggingService.TryShowBlogArticle(articleId, out _))
            {
                return this.NotFound();
            }

            if (!this.commentsManagementService.TryShowBlogComment(commentId, out var comment))
            {
                return comment;
            }

            return this.NotFound();
        }
    }
}
