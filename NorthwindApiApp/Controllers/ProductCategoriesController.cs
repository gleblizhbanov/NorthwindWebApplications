using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Northwind.Services.Models;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    /// <summary>
    /// Provides a controller to work with product categories.
    /// </summary>
    [Route("api/categories")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryManagementService productCategoryManagementService;
        private readonly IProductCategoryPictureManagementService productCategoryPictureManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoriesController"/> class.
        /// </summary>
        /// <param name="productCategoryManagementService">A product category management service.</param>
        /// <param name="productCategoryPictureManagementService">A product category picture management service.</param>
        public ProductCategoriesController(IProductCategoryManagementService productCategoryManagementService, IProductCategoryPictureManagementService productCategoryPictureManagementService)
        {
            this.productCategoryManagementService = productCategoryManagementService;
            this.productCategoryPictureManagementService = productCategoryPictureManagementService;
        }

        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="category">A product category to add.</param>
        /// <returns>An action result.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategoryAsync(Category category)
        {
            int id = await this.productCategoryManagementService.CreateCategoryAsync(category).ConfigureAwait(false);
            if (id > 0)
            {
                return this.CreatedAtAction("CreateCategory", new { id }, category);
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Gets all product categories.
        /// </summary>
        /// <returns>All product categories.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesAsync()
        {
            if (await this.productCategoryManagementService.ShowCategoriesAsync(0, int.MaxValue).ConfigureAwait(false) is not List<Category> categories)
            {
                return this.BadRequest();
            }

            return categories;
        }

        /// <summary>
        /// Gets the product category by id.
        /// </summary>
        /// <param name="id">An id of the category.</param>
        /// <returns>The product category with given id.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Category> GetCategory(int id)
        {
            if (this.productCategoryManagementService.TryShowCategory(id, out var productCategory))
            {
                return productCategory;
            }

            return this.NotFound();
        }

        /// <summary>
        /// Updates the category with specific id.
        /// </summary>
        /// <param name="id">An id of the category.</param>
        /// <param name="category">A new product category.</param>
        /// <returns>An action result.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategoryAsync(int id, Category category)
        {
            if (category is null || id != category.CategoryId)
            {
                return this.BadRequest();
            }

            if (await this.productCategoryManagementService.UpdateCategoryAsync(id, category).ConfigureAwait(false))
            {
                return this.Ok();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Deletes the product category.
        /// </summary>
        /// <param name="id">An id of the category.</param>
        /// <returns>An action result.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            if (await this.productCategoryManagementService.DestroyCategoryAsync(id).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Uploads a product category picture.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <param name="file">A picture file.</param>
        /// <returns>An action result.</returns>
        [HttpPut("{categoryId:int}/picture")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadPictureAsync(int categoryId, IFormFile file)
        {
            if (file is null || !await this.productCategoryPictureManagementService.UpdatePictureAsync(categoryId, file.OpenReadStream()).ConfigureAwait(false))
            {
                return this.BadRequest();
            }

            return this.NoContent();
        }

        /// <summary>
        /// Gets the product category picture.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>A product category picture file.</returns>
        [HttpGet("{categoryId:int}/picture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<byte[]> GetPicture(int categoryId)
        {
            if (this.productCategoryPictureManagementService.TryShowPicture(categoryId, out var bytes))
            {
                return this.File(bytes[78..], "image/jpg");
            }

            if (!this.productCategoryManagementService.TryShowCategory(categoryId, out _))
            {
                return this.NotFound();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Deletes the product category picture.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>An action result.</returns>
        [HttpDelete("{categoryId:int}/picture")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePictureAsync(int categoryId)
        {
            if (await this.productCategoryPictureManagementService.DestroyPictureAsync(categoryId).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            if (!this.productCategoryManagementService.TryShowCategory(categoryId, out _))
            {
                return this.NotFound();
            }

            return this.BadRequest();
        }
    }
}
