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
    /// Provides a management service using data access object for product categories.
    /// </summary>
    public class ProductCategoriesManagementDataAccessService : IProductCategoryManagementService
    {
        private readonly IProductCategoryDataAccessObject productCategoryDataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoriesManagementDataAccessService"/> class.
        /// </summary>
        /// <param name="dataAccessFactory">A Northwind data access factory.</param>
        public ProductCategoriesManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            this.productCategoryDataAccessObject = dataAccessFactory.GetProductCategoryDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<IList<Category>> ShowCategoriesAsync(int offset, int limit)
        {
            return (await this.productCategoryDataAccessObject.SelectProductCategoriesAsync(offset, limit).ConfigureAwait(false))
                                                              .Select(GetCategory)
                                                              .ToList();
        }

        /// <inheritdoc/>
        public bool TryShowCategory(int categoryId, out Category category)
        {
            var categoryTask = this.productCategoryDataAccessObject.FindProductCategoryAsync(categoryId);
            try
            {
                category = GetCategory(categoryTask.Result);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(e => e.GetType() == typeof(ProductCategoryNotFoundException)))
                {
                    category = null;
                    return false;
                }

                throw;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(Category category)
        {
            if (category is null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            return await this.productCategoryDataAccessObject.InsertProductCategoryAsync(GetCategoryDataTransferObject(category)).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            return await this.productCategoryDataAccessObject.DeleteProductCategoryAsync(categoryId).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IList<Category>> LookupCategoriesByNameAsync(IList<string> names)
        {
            return (await this.productCategoryDataAccessObject.SelectProductCategoriesByNameAsync(names).ConfigureAwait(false))
                                                              .Select(GetCategory)
                                                              .ToList();
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, Category category)
        {
            return await this.productCategoryDataAccessObject.UpdateProductCategoryAsync(GetCategoryDataTransferObject(category)).ConfigureAwait(false);
        }

        private static Category GetCategory(ProductCategoryTransferObject category) =>
            new ()
            {
                Description = category.Description,
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Picture = category.Picture,
            };

        private static ProductCategoryTransferObject GetCategoryDataTransferObject(Category category) =>
            new ()
            {
                Description = category.Description,
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Picture = category.Picture,
            };
    }
}
