using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    /// <summary>
    /// Represents a stub for a product management service.
    /// </summary>
    public sealed class ProductManagementService : IProductManagementService
    {
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="context">A db context to use.</param>
        public ProductManagementService(NorthwindContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            var entityEntry = await this.context.AddAsync(product);
            await this.context.SaveChangesAsync();
            return entityEntry.Entity.Id;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId)
        {
            var product = await this.context.FindAsync<Product>(productId);
            this.context.Remove(product);
            return await this.context.SaveChangesAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> LookupProductsByNameAsync(IList<string> names)
        {
            return await this.context.Products.Where(product => names.Contains(product.Name)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> ShowProductsAsync(int offset, int limit)
        {
            var products = this.context.Products.Skip(offset);
            if (limit != -1)
            {
                products = products.Skip(limit);
            }

            return await products.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> ShowProductsForCategoryAsync(int categoryId)
        {
            return await this.context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        /// <inheritdoc/>
        public bool TryShowProduct(int productId, out Product product)
        {
            product = this.context.Find<Product>(productId);
            return product is not null;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            if (!this.TryShowProduct(productId, out var oldProduct))
            {
                return false;
            }

            this.context.Entry(oldProduct).CurrentValues.SetValues(productId);
            this.context.Update(oldProduct);
            return await this.context.SaveChangesAsync() > 0;
        }
    }
}