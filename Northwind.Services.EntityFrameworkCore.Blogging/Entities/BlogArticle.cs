using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    /// <summary>
    /// Represents a blog article.
    /// </summary>
    [Table("Blog Articles")]
    public class BlogArticle
    {
        /// <summary>
        /// Gets or sets the article identifier.
        /// </summary>
        [Key]
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the article title.
        /// </summary>
        [StringLength(40)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the article text.
        /// </summary>
        [Required]
        [Column(TypeName = "ntext")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the article publication time.
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Posted { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [Required]
        public int AuthorId { get; set; }
    }
}
