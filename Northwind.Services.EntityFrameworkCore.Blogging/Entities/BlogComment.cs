using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    /// <summary>
    /// Represents a blog comment.
    /// </summary>
    [Table("Blog Comment")]
    public class BlogComment
    {
        /// <summary>
        /// Gets or sets the blog comment identifier.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets an identifier of the article the comment belongs to.
        /// </summary>
        [Column("ArticleId")]
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the time the comment was published.
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Published { get; set; }
    }
}
