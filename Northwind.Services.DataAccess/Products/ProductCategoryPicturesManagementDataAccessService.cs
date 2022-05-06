using System.IO;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.Products
{
    /// <summary>
    /// Provides a management service using data access object for product category pictures.
    /// </summary>
    public class ProductCategoryPicturesManagementDataAccessService : IProductCategoryPictureManagementService
    {
        private readonly IProductCategoryDataAccessObject productCategoryDataAccessObject;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductCategoriesManagementDataAccessService"/> class.
        /// </summary>
        /// <param name="dataAccessFactory">A Northwind data access factory.</param>
        public ProductCategoryPicturesManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            this.productCategoryDataAccessObject = dataAccessFactory.GetProductCategoryDataAccessObject();
        }

        /// <inheritdoc/>
        public bool TryShowPicture(int categoryId, out byte[] bytes)
        {
            Task<ProductCategoryTransferObject> categoryTask;
            try
            {
                categoryTask = this.productCategoryDataAccessObject.FindProductCategoryAsync(categoryId);
            }
            catch (ProductCategoryNotFoundException)
            {
                bytes = null;
                return false;
            }

            if (categoryTask is null)
            {
                bytes = null;
                return false;
            }

            categoryTask.Wait();
            bytes = categoryTask.Result.Picture;
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePictureAsync(int categoryId, Stream stream)
        {
            ProductCategoryTransferObject category;
            try
            {
                category = await this.productCategoryDataAccessObject.FindProductCategoryAsync(categoryId);
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }

            if (category is null)
            {
                return false;
            }

            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            category.Picture = memoryStream.ToArray();
            return await this.productCategoryDataAccessObject.UpdateProductCategoryAsync(category);
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPictureAsync(int categoryId)
        {
            ProductCategoryTransferObject category;
            try
            {
                category = await this.productCategoryDataAccessObject.FindProductCategoryAsync(categoryId);
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }

            if (category is null)
            {
                return false;
            }

            category.Picture = null;

            return true;
        }
    }
}
