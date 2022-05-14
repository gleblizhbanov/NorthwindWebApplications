using System;

namespace Northwind.Services.Models
{
    /// <summary>
    /// Represents a blog comment.
    /// </summary>
    public class BlogComment
    {
        /// <summary>
        /// Gets or sets the blog comment identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets an identifier of the article the comment belongs to.
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the time the comment was published.
        /// </summary>
        public DateTime? Published { get; set; }
    }
}
