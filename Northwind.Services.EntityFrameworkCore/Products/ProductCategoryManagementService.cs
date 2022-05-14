using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Products;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.EntityFrameworkCore.Entities;

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
        public async Task<int> CreateCategoryAsync(Models.Category category)
        {
            var entityEntry = await this.context.AddAsync(GetCategoryEntity(category)).ConfigureAwait(false);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return entityEntry.Entity.CategoryId;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            var category = await this.context.FindAsync<Category>(categoryId).ConfigureAwait(false);
            this.context.Remove(category);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public async Task<IList<Models.Category>> LookupCategoriesByNameAsync(IList<string> names) =>
            this.context.Categories.Where(category => names.Contains(category.CategoryName))
                                   .AsEnumerable()
                                   .Select(GetCategoryModel)
                                   .ToList();

        /// <inheritdoc/>
        public async Task<IList<Models.Category>> ShowCategoriesAsync(int offset, int limit) =>
            limit != int.MaxValue
                ? this.context.Categories.Skip(offset).Take(limit).AsEnumerable().Select(GetCategoryModel).ToList()
                : this.context.Categories.Skip(limit).AsEnumerable().Select(GetCategoryModel).ToList();

        /// <inheritdoc/>
        public bool TryShowCategory(int categoryId, out Models.Category category)
        {
            var categoryEntity = this.context.Find<Category>(categoryId);
            if (categoryEntity is null)
            {
                category = null;
                return false;
            }

            category = GetCategoryModel(categoryEntity);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, Models.Category productCategory)
        {
            var category = await this.context.FindAsync<Category>(categoryId).ConfigureAwait(false);
            if (category is null)
            {
                return false;
            }

            category.CategoryId = productCategory.CategoryId;
            category.CategoryName = productCategory.CategoryName;
            category.Description = productCategory.Description;
            category.Picture = productCategory.Picture;
            this.context.Update(category);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        private static Category GetCategoryEntity(Models.Category category) =>
            new ()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Picture = category.Picture,
            };

        private static Models.Category GetCategoryModel(Category category) =>
            new ()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Picture = category.Picture,
            };
    }
}
