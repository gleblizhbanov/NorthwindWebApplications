using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
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
        /// Initializes a new instance of <see cref="ProductManagementDataAccessService"/> class.
        /// </summary>
        /// <param name="dataAccessFactory">A Northwind data access factory.</param>
        public ProductManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            this.productDataAccessObject = dataAccessFactory.GetProductDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> ShowProductsAsync(int offset, int limit)
        {
            return (await this.productDataAccessObject.SelectProductsAsync(offset, limit)).Select(GetProduct).ToList();
        }

        /// <inheritdoc/>
        public bool TryShowProduct(int productId, out Product product)
        {
            Task<ProductTransferObject> productTransferObjectTask;
            try
            {
                productTransferObjectTask = this.productDataAccessObject.FindProductAsync(productId);
            }
            catch (ProductNotFoundException)
            {
                product = null;
                return false;
            }

            if (productTransferObjectTask is null)
            {
                product = null;
                return false;
            }
            
            product = GetProduct(productTransferObjectTask.Result);

            return true;
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            return await this.productDataAccessObject.InsertProductAsync(GetProductTransferObject(product));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId)
        {
            return await this.productDataAccessObject.DeleteProductAsync(productId);
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> LookupProductsByNameAsync(IList<string> names)
        {
            return (await this.productDataAccessObject.SelectProductsByNameAsync(names)).Select(GetProduct).ToList();
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            return await this.productDataAccessObject.UpdateProductAsync(GetProductTransferObject(product));
        }

        /// <inheritdoc/>
        public async Task<IList<Product>> ShowProductsForCategoryAsync(int categoryId)
        {
            return (await this.productDataAccessObject.SelectProductByCategoryAsync(new[] {categoryId})).Select(GetProduct).ToList();
        }

        private static Product GetProduct(ProductTransferObject product) =>
            new()
            {
                CategoryId = product.CategoryId,
                Discontinued = product.Discontinued,
                Id = product.Id,
                Name = product.Name,
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
                Id = product.Id,
                Name = product.Name,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.CategoryId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
            };
    }
}
