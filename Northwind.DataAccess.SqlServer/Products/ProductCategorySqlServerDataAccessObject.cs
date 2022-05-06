using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using Northwind.DataAccess.Products;

namespace Northwind.DataAccess.SqlServer.Products
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind product categories.
    /// </summary>
    public sealed class ProductCategorySqlServerDataAccessObject : IProductCategoryDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategorySqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductCategorySqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProductCategoryAsync(ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            const string commandText =
@"INSERT INTO dbo.Categories (CategoryName, Description, Picture) OUTPUT Inserted.CategoryID
VALUES (@categoryName, @description, @picture)";

            await using var command = new SqlCommand(commandText, this.connection);
            AddSqlParameters(productCategory, command);
            await this.connection.OpenAsync();
            var id = await command.ExecuteScalarAsync();
            await this.connection.CloseAsync();
            return (int)id;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProductCategoryAsync(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            const string commandText =
@"DELETE FROM dbo.Categories WHERE CategoryID = @categoryID
SELECT @@ROWCOUNT";

            await using var command = new SqlCommand(commandText, this.connection);

            const string categoryId = "@categoryID";
            command.Parameters.Add(categoryId, SqlDbType.Int);
            command.Parameters[categoryId].Value = productCategoryId;
            
            await this.connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            await this.connection.CloseAsync();
            return (int)result > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductCategoryTransferObject> FindProductCategoryAsync(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            const string commandText =
@"SELECT c.CategoryID, c.CategoryName, c.Description, c.Picture FROM dbo.Categories as c
WHERE c.CategoryID = @categoryId";

            await using var command = new SqlCommand(commandText, this.connection);
            const string categoryId = "@categoryId";
            command.Parameters.Add(categoryId, SqlDbType.Int);
            command.Parameters[categoryId].Value = productCategoryId;

            await this.connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            if (!await reader.ReadAsync())
            {
                throw new ProductCategoryNotFoundException(productCategoryId);
            }

            return CreateProductCategory(reader);
        }

        /// <inheritdoc/>
        public async Task<IList<ProductCategoryTransferObject>> SelectProductCategoriesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            const string commandTemplate =
@"SELECT c.CategoryID, c.CategoryName, c.Description, c.Picture FROM dbo.Categories as c
ORDER BY c.CategoryID
OFFSET {0} ROWS
FETCH FIRST {1} ROWS ONLY";

            string commandText = string.Format(CultureInfo.CurrentCulture, commandTemplate, offset, limit);
            return await this.ExecuteReaderAsync(commandText);
        }

        /// <inheritdoc/>
        public async Task<IList<ProductCategoryTransferObject>> SelectProductCategoriesByNameAsync(ICollection<string> productCategoryNames)
        {
            if (productCategoryNames == null)
            {
                throw new ArgumentNullException(nameof(productCategoryNames));
            }

            if (productCategoryNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productCategoryNames));
            }

            const string commandTemplate =
@"SELECT c.CategoryID, c.CategoryName, c.Description, c.Picture FROM dbo.Categories as c
WHERE c.CategoryName in ('{0}')
ORDER BY c.CategoryID";

            string commandText = string.Format(CultureInfo.CurrentCulture, commandTemplate, string.Join("', '", productCategoryNames));
            return await this.ExecuteReaderAsync(commandText);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductCategoryAsync(ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            const string commandText =
@"UPDATE dbo.Categories SET CategoryName = @categoryName, Description = @description, Picture = @picture
WHERE CategoryID = @categoryId
SELECT @@ROWCOUNT";

            await using var command = new SqlCommand(commandText, this.connection);
            AddSqlParameters(productCategory, command);

            const string categoryId = "@categoryId";
            command.Parameters.Add(categoryId, SqlDbType.Int);
            command.Parameters[categoryId].Value = productCategory.Id;

            await this.connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            await this.connection.CloseAsync();
            return (int)result > 0;
        }

        private static ProductCategoryTransferObject CreateProductCategory(SqlDataReader reader)
        {
            var id = (int)reader["CategoryID"];
            var name = (string)reader["CategoryName"];

            const string descriptionColumnName = "Description";
            string description = null;

            if (reader[descriptionColumnName] != DBNull.Value)
            {
                description = (string)reader["Description"];
            }

            const string pictureColumnName = "Picture";
            byte[] picture = null;

            if (reader[pictureColumnName] != DBNull.Value)
            {
                picture = (byte[])reader["Picture"];
            }

            return new ProductCategoryTransferObject
            {
                Id = id,
                Name = name,
                Description = description,
                Picture = picture,
            };
        }

        private static void AddSqlParameters(ProductCategoryTransferObject productCategory, SqlCommand command)
        {
            const string categoryNameParameter = "@categoryName";
            command.Parameters.Add(categoryNameParameter, SqlDbType.NVarChar, 15);
            command.Parameters[categoryNameParameter].Value = productCategory.Name;

            const string descriptionParameter = "@description";
            command.Parameters.Add(descriptionParameter, SqlDbType.NText);
            command.Parameters[descriptionParameter].IsNullable = true;

            if (productCategory.Description != null)
            {
                command.Parameters[descriptionParameter].Value = productCategory.Description;
            }
            else
            {
                command.Parameters[descriptionParameter].Value = DBNull.Value;
            }

            const string pictureParameter = "@picture";
            command.Parameters.Add(pictureParameter, SqlDbType.Image);
            command.Parameters[pictureParameter].IsNullable = true;

            if (productCategory.Picture != null)
            {
                command.Parameters[pictureParameter].Value = productCategory.Picture;
            }
            else
            {
                command.Parameters[pictureParameter].Value = DBNull.Value;
            }
        }

        private async Task<IList<ProductCategoryTransferObject>> ExecuteReaderAsync(string commandText)
        {
            var productCategories = new List<ProductCategoryTransferObject>();

            await using var command = new SqlCommand(commandText, this.connection);
            await this.connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await reader.ReadAsync())
            {
                productCategories.Add(CreateProductCategory(reader));
            }

            return productCategories;
        }
    }
}