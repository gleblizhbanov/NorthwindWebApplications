namespace Northwind.Services.Models
{
    /// <summary>
    /// Represents a blog article product.
    /// </summary>
    public class BlogArticleProduct
    {
        /// <summary>
        /// Gets or sets the blog article product identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the blog article identifier.
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        public int ProductId { get; set; }
    }
}
