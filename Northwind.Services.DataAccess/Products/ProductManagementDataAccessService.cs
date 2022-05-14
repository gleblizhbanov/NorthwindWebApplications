using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Models;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.Products
{
    /// <summary>
    /// Provides a management service using data access object for products.
    /// </summary>
    public class ProductManagementDataAccessService : IProductManagementService
    {
        private readonly IProductDataAccessObject productDataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementDataAccessService"/> class.
        /// </summary>
        /// <param name="dataAccessFactory">A Northwind data access factory.</param>
        public ProductManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            this.productDataAccessObject = dataAccessFactory.GetProductDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> ShowProductsAsync(int offset, int limit)
        {
            return (await this.productDataAccessObject.SelectProductsAsync(offset, limit).ConfigureAwait(false))
                                                      .Select(GetProduct)
                                                      .ToList();
        }

        /// <inheritdoc/>
        public bool TryShowProduct(int productId, out Product product)
        {
            var productTransferObjectTask = this.productDataAccessObject.FindProductAsync(productId);
            try
            {
                product = GetProduct(productTransferObjectTask.Result);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(e => e.GetType() == typeof(ProductNotFoundException)))
                {
                    product = null;
                    return false;
                }

                throw;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            return await this.productDataAccessObject.InsertProductAsync(GetProductTransferObject(product)).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId)
        {
            return await this.productDataAccessObject.DeleteProductAsync(productId).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> LookupProductsByNameAsync(IList<string> names)
        {
            return (await this.productDataAccessObject.SelectProductsByNameAsync(names).ConfigureAwait(false))
                                                      .Select(GetProduct)
                                                      .ToList();
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            return await this.productDataAccessObject.UpdateProductAsync(GetProductTransferObject(product)).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> ShowProductsForCategoryAsync(int categoryId)
        {
            return (await this.productDataAccessObject.SelectProductByCategoryAsync(new[] { categoryId }).ConfigureAwait(false))
                                                      .Select(GetProduct)
                                                      .ToList();
        }

        private static Product GetProduct(ProductTransferObject product) =>
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

        private static ProductTransferObject GetProductTransferObject(Product product) =>
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
