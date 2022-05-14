using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Northwind.Services.Blogging;
using Northwind.Services.Employees;
using Northwind.Services.Models;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    /// <summary>
    /// Represents a controller to work with blog articles.
    /// </summary>
    [Route("api/articles")]
    [ApiController]
    public class BlogArticlesController : ControllerBase
    {
        private readonly IBloggingService bloggingService;
        private readonly IBlogArticleProductsManagementService blogArticleProductsManagementService;
        private readonly IEmployeeManagementService employeeManagementService;
        private readonly IProductManagementService productManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogArticlesController"/> class.
        /// </summary>
        /// <param name="bloggingService">A blogging service.</param>
        /// <param name="blogArticleProductsManagementService">A blog article products management service.</param>
        /// <param name="employeeManagementService">An employee management service.</param>
        public BlogArticlesController(IBloggingService bloggingService, IBlogArticleProductsManagementService blogArticleProductsManagementService, IEmployeeManagementService employeeManagementService, IProductManagementService productManagementService)
        {
            this.bloggingService = bloggingService;
            this.blogArticleProductsManagementService = blogArticleProductsManagementService;
            this.employeeManagementService = employeeManagementService;
            this.productManagementService = productManagementService;
        }

        /// <summary>
        /// Creates a new blog article.
        /// </summary>
        /// <param name="article">A blog article to add.</param>
        /// <returns>An action result.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateArticleAsync(BlogArticle article)
        {
            if (!this.employeeManagementService.TryShowEmployee(article.AuthorId, out _))
            {
                return this.BadRequest();
            }

            article.Posted = DateTime.Now;
            int id = await this.bloggingService.CreateBlogArticleAsync(article).ConfigureAwait(false);
            if (id > 0)
            {
                return this.CreatedAtAction("CreateArticle", new { id }, article);
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Deletes a blog article with specific identifier.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <returns>An action result.</returns>
        [HttpDelete("{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteArticleAsync(int articleId)
        {
            if (await this.bloggingService.DeleteBlogArticleAsync(articleId).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Return the list of blog articles.
        /// </summary>
        /// <returns>A list of blog articles.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IList<object>>> GetBlogArticlesAsync()
        {
            if (await this.bloggingService.ShowBlogArticlesAsync(0, int.MaxValue).ConfigureAwait(false) is not List<BlogArticle> articles)
            {
                return this.BadRequest();
            }

            return articles.Select(this.GetArticleView).ToList();
        }

        /// <summary>
        /// Returns a blog article with specific identifier.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <returns>A blog article.</returns>
        [HttpGet("{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<object> GetBlogArticle(int articleId)
        {
            if (this.bloggingService.TryShowBlogArticle(articleId, out var article))
            {
                return this.GetArticleByIdView(article);
            }

            return this.NotFound();
        }

        /// <summary>
        /// Updates a blog article with specific identifier.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="article">A blog article to replace with.</param>
        /// <returns>An action result.</returns>
        [HttpPut("{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBlogArticleAsync(int articleId, BlogArticle article)
        {
            if (article is null)
            {
                return this.BadRequest();
            }

            article.Posted = DateTime.Now;
            if (await this.bloggingService.UpdateBlogArticleAsync(articleId, article).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Returns all products linked to a blog article.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <returns>A list of blog article products.</returns>
        [HttpGet("{articleId:int}/products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IList<Product>>> GetBlogArticleProductsAsync(int articleId)
        {
            if (await this.blogArticleProductsManagementService.ShowBlogArticleProductsAsync(articleId, 0, int.MaxValue).ConfigureAwait(false) is not List<BlogArticleProduct> articleProducts)
            {
                return this.BadRequest();
            }

            var list = new List<Product>();
            foreach (var articleProduct in articleProducts)
            {
                if (this.productManagementService.TryShowProduct(articleProduct.ProductId, out var product))
                {
                    list.Add(product);
                }
            }

            return list;
        }

        /// <summary>
        /// Adds a link to a product from a blog article.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="productId">A product identifier.</param>
        /// <returns>An action result.</returns>
        [HttpPost("{articleId:int}/products/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBlogArticleProductAsync(int articleId, int productId)
        {
            if (!this.bloggingService.TryShowBlogArticle(articleId, out _))
            {
                return this.NotFound();
            }

            if (await this.blogArticleProductsManagementService.CreateBlogArticleProductLink(articleId, productId).ConfigureAwait(false))
            {
                return this.CreatedAtAction("AddBlogArticleProduct", new { articleId, productId });
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Deletes a link to a product from a blog article.
        /// </summary>
        /// <param name="articleId">A blog article identifier.</param>
        /// <param name="productId">A product identifier.</param>
        /// <returns>An action result.</returns>
        [HttpDelete("{articleId:int}/products/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveBlogArticleProduct(int articleId, int productId)
        {
            if (!this.bloggingService.TryShowBlogArticle(articleId, out _))
            {
                return this.NotFound();
            }

            if (await this.blogArticleProductsManagementService.RemoveBlogArticleProductLink(articleId, productId).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }

        private object GetArticleView(BlogArticle article)
        {
            this.employeeManagementService.TryShowEmployee(article.AuthorId, out var author);
            return new
            {
                Id = article.ArticleId,
                article.Title,
                article.Posted,
                article.AuthorId,
                AuthorName = $"{author.FirstName} {author.LastName}, {author.Title}",
            };
        }

        private object GetArticleByIdView(BlogArticle article)
        {
            this.employeeManagementService.TryShowEmployee(article.AuthorId, out var author);
            return new
            {
                Id = article.ArticleId,
                article.Title,
                article.Posted,
                article.AuthorId,
                AuthorName = $"{author.FirstName} {author.LastName}, {author.Title}",
                article.Text,
            };
        }
    }
}
