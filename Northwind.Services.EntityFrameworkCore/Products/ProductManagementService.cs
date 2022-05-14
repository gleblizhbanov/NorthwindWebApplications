using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Products;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.EntityFrameworkCore.Entities;

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
        public async Task<int> CreateProductAsync(Models.Product product)
        {
            var entityEntry = await this.context.AddAsync(GetProductEntity(product)).ConfigureAwait(false);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return entityEntry.Entity.ProductId;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId)
        {
            var product = await this.context.FindAsync<Product>(productId).ConfigureAwait(false);
            this.context.Remove(product);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public async Task<IList<Models.Product>> LookupProductsByNameAsync(IList<string> names) => 
            this.context.Products.Where(product => names.Contains(product.ProductName))
                                 .AsEnumerable()
                                 .Select(GetProductModel)
                                 .ToList();

        /// <inheritdoc/>
        public async Task<IList<Models.Product>> ShowProductsAsync(int offset, int limit) =>
            limit != int.MaxValue
                ? this.context.Products.Skip(offset).Take(limit).AsEnumerable().Select(GetProductModel).ToList()
                : this.context.Products.Skip(offset).AsEnumerable().Select(GetProductModel).ToList();

        /// <inheritdoc/>
        public async Task<IList<Models.Product>> ShowProductsForCategoryAsync(int categoryId) =>
            this.context.Products.Where(p => p.CategoryId == categoryId)
                                 .AsEnumerable()
                                 .Select(GetProductModel)
                                 .ToList();

        /// <inheritdoc/>
        public bool TryShowProduct(int productId, out Models.Product product)
        {
            var productEntity = this.context.Find<Product>(productId);
            if (productEntity is null)
            {
                product = null;
                return false;
            }

            product = GetProductModel(productEntity);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Models.Product product)
        {
            var productEntity = await this.context.FindAsync<Product>(productId).ConfigureAwait(false);
            if (productEntity is null)
            {
                return false;
            }

            productEntity.CategoryId = product.CategoryId;
            productEntity.Discontinued = product.Discontinued;
            productEntity.ProductId = product.ProductId;
            productEntity.ProductName = product.ProductName;
            productEntity.QuantityPerUnit = product.QuantityPerUnit;
            productEntity.ReorderLevel = product.ReorderLevel;
            productEntity.SupplierId = product.CategoryId;
            productEntity.UnitPrice = product.UnitPrice;
            productEntity.UnitsInStock = product.UnitsInStock;
            productEntity.UnitsOnOrder = product.UnitsOnOrder;
            this.context.Update(productEntity);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        private static Models.Product GetProductModel(Product product) =>
            new()
            {
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.CategoryId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
            };

        private static Product GetProductEntity(Models.Product product) =>
            new()
            {
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.CategoryId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
            };
    }
}