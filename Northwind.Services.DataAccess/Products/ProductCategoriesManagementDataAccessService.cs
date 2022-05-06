using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.Products
{
    /// <summary>
    /// Provides a management service using data access object for product categories.
    /// </summary>
    public class ProductCategoriesManagementDataAccessService : IProductCategoryManagementService
    {
        private readonly IProductCategoryDataAccessObject productCategoryDataAccessObject;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductCategoryPicturesManagementDataAccessService"/> class.
        /// </summary>
        /// <param name="dataAccessFactory">A Northwind data access factory.</param>
        public ProductCategoriesManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            this.productCategoryDataAccessObject = dataAccessFactory.GetProductCategoryDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<IList<ProductCategory>> ShowCategoriesAsync(int offset, int limit)
        {
            return (await this.productCategoryDataAccessObject.SelectProductCategoriesAsync(offset, limit)).Select(GetProductCategory).ToList();
        }

        /// <inheritdoc/>
        public bool TryShowCategory(int categoryId, out ProductCategory productCategory)
        {
            Task<ProductCategoryTransferObject> categoryTask;
            try
            {
                categoryTask = this.productCategoryDataAccessObject.FindProductCategoryAsync(categoryId);
            }
            catch (ProductCategoryNotFoundException)
            {
                productCategory = null;
                return false;
            }

            if (categoryTask is null)
            {
                productCategory = null;
                return false;
            }
            
            productCategory = GetProductCategory(categoryTask.Result);

            return true;
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            return await this.productCategoryDataAccessObject.InsertProductCategoryAsync(GetProductCategoryDataTransferObject(productCategory));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            return await this.productCategoryDataAccessObject.DeleteProductCategoryAsync(categoryId);
        }

        /// <inheritdoc/>
        public async Task<IList<ProductCategory>> LookupCategoriesByNameAsync(IList<string> names)
        {
            return (await this.productCategoryDataAccessObject.SelectProductCategoriesByNameAsync(names)).Select(GetProductCategory).ToList();
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            return await this.productCategoryDataAccessObject.UpdateProductCategoryAsync(GetProductCategoryDataTransferObject(productCategory));
        }

        private static ProductCategory GetProductCategory(ProductCategoryTransferObject category) =>
            new()
            {
                Description = category.Description,
                Id = category.Id,
                Name = category.Name,
                Picture = category.Picture,
            };

        private static ProductCategoryTransferObject GetProductCategoryDataTransferObject(ProductCategory category) =>
            new()
            {
                Description = category.Description,
                Id = category.Id,
                Name = category.Name,
                Picture = category.Picture,
            };
    }
}
