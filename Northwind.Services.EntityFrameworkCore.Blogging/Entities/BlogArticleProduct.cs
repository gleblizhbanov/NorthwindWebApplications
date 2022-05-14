using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    /// <summary>
    /// Represents a blog article product.
    /// </summary>
    [Table("Blog Article Products")]
    public class BlogArticleProduct
    {
        /// <summary>
        /// Gets or sets the blog article product identifier.
        /// </summary>
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the blog article identifier.
        /// </summary>
        [Column("ArticleId")]
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        [Column("ProductId")]
        public int ProductId { get; set; }
    }
}
