using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Products;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.EntityFrameworkCore.Entities;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    /// <summary>
    /// Represents a stub for a product category picture management service.
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
            var category = await this.context.FindAsync<Category>(categoryId).ConfigureAwait(false);
            if (category is null)
            {
                return false;
            }

            category.Picture = null;
            this.context.Update(category);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public bool TryShowPicture(int categoryId, out byte[] bytes)
        {
            var category = this.context.Find<Category>(categoryId);
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
            var category = await this.context.FindAsync<Category>(categoryId).ConfigureAwait(false);
            if (category is null)
            {
                return false;
            }

            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            category.Picture = memoryStream.ToArray();
            this.context.Update(category);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }
    }
}
