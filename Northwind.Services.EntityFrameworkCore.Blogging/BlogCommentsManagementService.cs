using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Blogging;
using Northwind.Services.EntityFrameworkCore.Blogging.Context;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;

namespace Northwind.Services.EntityFrameworkCore.Blogging
{
    /// <summary>
    /// Provides a blog comments management service.
    /// </summary>
    public class BlogCommentsManagementService : IBlogCommentsManagementService
    {
        private readonly BloggingContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogCommentsManagementService"/> class.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public BlogCommentsManagementService(string[] args)
        {
            this.context = new DesignTimeBloggingContextFactory().CreateDbContext(args);
        }

        /// <inheritdoc/>
        public async Task<int> CreateBlogCommentAsync(Models.BlogComment comment)
        {
            var entity = await this.context.AddAsync(GetBlogCommentEntity(comment)).ConfigureAwait(false);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return entity.Entity.Id;
        }

        /// <inheritdoc/>
        public async Task<IList<Models.BlogComment>> ShowBlogCommentsAsync(int articleId, int offset, int limit) =>
            limit != int.MaxValue
                ? this.context.BlogComments.Where(comment => comment.ArticleId == articleId).Skip(offset).Take(limit).AsEnumerable().Select(GetBlogCommentModel).ToList()
                : this.context.BlogComments.Where(comment => comment.ArticleId == articleId).Skip(offset).AsEnumerable().Select(GetBlogCommentModel).ToList();

        /// <inheritdoc/>
        public bool TryShowBlogComment(int commentId, out Models.BlogComment comment)
        {
            var commentEntity = this.context.Find<BlogComment>(commentId);
            if (commentEntity is null)
            {
                comment = null;
                return false;
            }

            comment = GetBlogCommentModel(commentEntity);
            return true;
        }


        /// <inheritdoc/>
        public async Task<bool> UpdateBlogCommentAsync(int commentId, Models.BlogComment comment)
        {
            var entity = await this.context.FindAsync<BlogComment>(commentId).ConfigureAwait(false);
            if (entity is null)
            {
                return false;
            }

            entity.Text = comment.Text;
            entity.Published = comment.Published;
            this.context.Update(entity);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteBlogCommentAsync(int commentId)
        {
            var entity = await this.context.FindAsync<BlogComment>(commentId).ConfigureAwait(false);
            this.context.Remove(entity);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        private static BlogComment GetBlogCommentEntity(Models.BlogComment comment) =>
            new ()
            {
                Id = comment.Id,
                ArticleId = comment.ArticleId,
                Text = comment.Text,
                Published = comment.Published,
            };

        private static Models.BlogComment GetBlogCommentModel(BlogComment comment) =>
            new ()
            {
                Id = comment.Id,
                ArticleId = comment.ArticleId,
                Text = comment.Text,
                Published = comment.Published,
            };
    }
}
