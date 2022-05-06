using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    /// <summary>
    /// Represents a stub for a product category management service.
    /// </summary>
    public class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="context">A db context.</param>
        public ProductCategoryManagementService(NorthwindContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            var entityEntry = await this.context.AddAsync(productCategory);
            await this.context.SaveChangesAsync();
            return entityEntry.Entity.Id;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            var category = await this.context.FindAsync<ProductCategory>(categoryId);
            this.context.Remove(category);
            return await this.context.SaveChangesAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<IList<ProductCategory>> LookupCategoriesByNameAsync(IList<string> names)
        {
            return this.context.Categories.Where(category => names.Contains(category.Name)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IList<ProductCategory>> ShowCategoriesAsync(int offset, int limit)
        {
            var categories = this.context.Categories.Skip(offset);
            if (limit != -1)
            {
                categories = categories.Take(limit);
            }

            return categories.ToList();
        }

        /// <inheritdoc/>
        public bool TryShowCategory(int categoryId, out ProductCategory productCategory)
        {
            productCategory = this.context.Find<ProductCategory>(categoryId);
            return productCategory is not null;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            if (!this.TryShowCategory(categoryId, out var category))
            {
                return false;
            }

            this.context.Entry(category).CurrentValues.SetValues(productCategory);
            this.context.Update(category);
            return await this.context.SaveChangesAsync() > 0;
        }
    }
}
