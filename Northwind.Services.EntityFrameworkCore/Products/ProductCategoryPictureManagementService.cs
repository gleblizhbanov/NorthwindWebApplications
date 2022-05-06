using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    /// <summary>
    /// Represents a stub for a product category management service.
    /// </summary>
    public class ProductCategoryPictureManagementService : IProductCategoryPictureManagementService
    {
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryPictureManagementService"/> class.
        /// </summary>
        /// <param name="context">A db content.</param>
        public ProductCategoryPictureManagementService(NorthwindContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPictureAsync(int categoryId)
        {
            var category = await this.context.FindAsync<ProductCategory>(categoryId);
            category.Picture = null;
            this.context.Update(category);
            return await this.context.SaveChangesAsync() > 0;
        }

        /// <inheritdoc/>
        public bool TryShowPicture(int categoryId, out byte[] bytes)
        {
            var category = this.context.Find<ProductCategory>(categoryId);
            if (category is null)
            {
                bytes = null;
                return false;
            }

            bytes = category.Picture;
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePictureAsync(int categoryId, Stream stream)
        {
            var category = await this.context.FindAsync<ProductCategory>(categoryId);
            if (category is null)
            {
                return false;
            }

            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            category.Picture = memoryStream.ToArray();

            return true;
        }
    }
}
