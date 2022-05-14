using System;

namespace Northwind.Services.Models
{
    /// <summary>
    /// Represents a blog article.
    /// </summary>
    public class BlogArticle
    {
        /// <summary>
        /// Gets or sets the article identifier.
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the article title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the article text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the article publication time.
        /// </summary>
        public DateTime? Posted { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public int AuthorId { get; set; }
    }
}
