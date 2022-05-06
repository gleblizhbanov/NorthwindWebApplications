using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    /// <summary>
    /// Represents a controller to work with products.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManagementService productManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="productManagementService">A product management service.</param>
        public ProductsController(IProductManagementService productManagementService)
        {
            this.productManagementService = productManagementService;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">A product to add.</param>
        /// <returns>An action result.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProductAsync(Product product)
        {
            if (await this.productManagementService.CreateProductAsync(product).ConfigureAwait(false) <= 0)
            {
                return this.BadRequest();
            }

            return this.CreatedAtAction(nameof(this.CreateProductAsync), new { id = product.Id }, product);
        }

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>All products.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync()
        {
            if (await this.productManagementService.ShowProductsAsync(0, int.MaxValue).ConfigureAwait(false) is not List<Product> products)
            {
                return this.BadRequest();
            }

            return products;
        }

        /// <summary>
        /// Gets the product by id.
        /// </summary>
        /// <param name="id">An id of the product.</param>
        /// <returns>The product with the given id.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProductAsync(int id)
        {
            if (this.productManagementService.TryShowProduct(id, out var product))
            {
                return product;
            }

            return this.NotFound();
        }

        /// <summary>
        /// Updates the product with specific id.
        /// </summary>
        /// <param name="id">An id of the product.</param>
        /// <param name="product">A new product.</param>
        /// <returns>An action result.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductAsync(int id, Product product)
        {
            if (product is null || id != product.Id)
            {
                return this.BadRequest();
            }

            if (await this.productManagementService.UpdateProductAsync(id, product).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.NoContent();
        }

        /// <summary>
        /// Deletes the product with specific id.
        /// </summary>
        /// <param name="id">An id of the product.</param>
        /// <returns>An action result.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            if (await this.productManagementService.DestroyProductAsync(id).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }
    }
}
