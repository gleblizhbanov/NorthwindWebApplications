namespace Northwind.Services.Models
{
    /// <summary>
    /// Represents a product category.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets a product category identifier.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets a product category name.
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets a product category description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a product category picture.
        /// </summary>
        public byte[] Picture { get; set; }
    }
}