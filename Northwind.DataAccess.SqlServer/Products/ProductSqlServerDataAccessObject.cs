using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Northwind.DataAccess.Products;

namespace Northwind.DataAccess.SqlServer.Products
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class ProductSqlServerDataAccessObject : IProductDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProductAsync(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await using var command = new SqlCommand("InsertProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(product, command);

            await this.connection.OpenAsync();
            var id = await command.ExecuteScalarAsync();
            await this.connection.CloseAsync();
            return (int)id;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            await using var command = new SqlCommand("DeleteProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productIdParameter = "@productID";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productId;

            await this.connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            await this.connection.CloseAsync();
            return (int)result > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductTransferObject> FindProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            await using var command = new SqlCommand("FindProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productIdParameter = "@productId";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productId;

            await this.connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            if (!await reader.ReadAsync())
            {
                throw new ProductNotFoundException(productId);
            }

            return CreateProduct(reader);
        }

        /// <inheritdoc />
        public async Task<IList<ProductTransferObject>> SelectProductsAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await using var sqlCommand = new SqlCommand("SelectProducts", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string offsetParameter = "@offset";
            sqlCommand.Parameters.Add(offsetParameter, SqlDbType.Int);
            sqlCommand.Parameters[offsetParameter].Value = offset;
            const string limitParameter = "@limit";
            sqlCommand.Parameters.Add(limitParameter, SqlDbType.Int);
            sqlCommand.Parameters[limitParameter].Value = limit;

            return await this.ExecuteReaderAsync(sqlCommand);
        }

        /// <inheritdoc/>
        public async Task<IList<ProductTransferObject>> SelectProductsByNameAsync(ICollection<string> productNames)
        {
            if (productNames == null)
            {
                throw new ArgumentNullException(nameof(productNames));
            }

            if (productNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productNames));
            }

            var products = string.Join("', '", productNames);
            await using var sqlCommand = new SqlCommand("SelectProductsByName", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productNamesParameter = "@productNames";
            sqlCommand.Parameters.Add(productNamesParameter, SqlDbType.NVarChar, 255);
            sqlCommand.Parameters[productNamesParameter].Value = string.Join(", ", productNames);

            return await this.ExecuteReaderAsync(sqlCommand);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await using var command = new SqlCommand("UpdateProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            
            AddSqlParameters(product, command);

            const string productId = "@productId";
            command.Parameters.Add(productId, SqlDbType.Int);
            command.Parameters[productId].Value = product.Id;

            await this.connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            await this.connection.CloseAsync();
            return (int)result > 0;
        }

        /// <inheritdoc/>
        public async Task<IList<ProductTransferObject>> SelectProductByCategoryAsync(ICollection<int> collectionOfCategoryId)
        {
            if (collectionOfCategoryId == null)
            {
                throw new ArgumentNullException(nameof(collectionOfCategoryId));
            }

            var whereInClause = string.Join("','", collectionOfCategoryId.Select(id => string.Format(CultureInfo.InvariantCulture, "{0:d}", id)));

            await using var sqlCommand = new SqlCommand("SelectProductsByCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productCategoriesParameter = "@productCategories";
            sqlCommand.Parameters.Add(productCategoriesParameter, SqlDbType.NVarChar, 255);
            sqlCommand.Parameters[productCategoriesParameter].Value = whereInClause;
            var products = new List<ProductTransferObject>();
            await this.connection.OpenAsync();
            await using var reader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await reader.ReadAsync())
            {
                products.Add(CreateProduct(reader));
            }

            return products;
        }

        private static ProductTransferObject CreateProduct(SqlDataReader reader)
        {
            var id = (int)reader["ProductID"];
            var name = (string)reader["ProductName"];

            const string supplierIdColumnName = "SupplierID";
            int? supplierId;

            if (reader[supplierIdColumnName] != DBNull.Value)
            {
                supplierId = (int)reader[supplierIdColumnName];
            }
            else
            {
                supplierId = null;
            }

            const string categoryIdColumnName = "CategoryID";
            int? categoryId;

            if (reader[categoryIdColumnName] != DBNull.Value)
            {
                categoryId = (int)reader[categoryIdColumnName];
            }
            else
            {
                categoryId = null;
            }

            const string quantityPerUnitColumnName = "QuantityPerUnit";
            string quantityPerUnit;

            if (reader[quantityPerUnitColumnName] != DBNull.Value)
            {
                quantityPerUnit = (string)reader[quantityPerUnitColumnName];
            }
            else
            {
                quantityPerUnit = null;
            }

            const string unitPriceColumnName = "UnitPrice";
            decimal? unitPrice;

            if (reader[unitPriceColumnName] != DBNull.Value)
            {
                unitPrice = (decimal)reader[unitPriceColumnName];
            }
            else
            {
                unitPrice = null;
            }

            const string unitsInStockColumnName = "UnitsInStock";
            short? unitsInStock;

            if (reader[unitsInStockColumnName] != DBNull.Value)
            {
                unitsInStock = (short)reader[unitsInStockColumnName];
            }
            else
            {
                unitsInStock = null;
            }

            const string unitsOnOrderColumnName = "UnitsOnOrder";
            short? unitsOnOrder;

            if (reader[unitsOnOrderColumnName] != DBNull.Value)
            {
                unitsOnOrder = (short)reader[unitsOnOrderColumnName];
            }
            else
            {
                unitsOnOrder = null;
            }

            const string reorderLevelColumnName = "ReorderLevel";
            short? reorderLevel;

            if (reader[reorderLevelColumnName] != DBNull.Value)
            {
                reorderLevel = (short)reader[reorderLevelColumnName];
            }
            else
            {
                reorderLevel = null;
            }

            const string discontinuedColumnName = "Discontinued";
            bool discontinued = (bool)reader[discontinuedColumnName];

            return new ProductTransferObject
            {
                Id = id,
                Name = name,
                SupplierId = supplierId,
                CategoryId = categoryId,
                QuantityPerUnit = quantityPerUnit,
                UnitPrice = unitPrice,
                UnitsInStock = unitsInStock,
                UnitsOnOrder = unitsOnOrder,
                ReorderLevel = reorderLevel,
                Discontinued = discontinued,
            };
        }

        private static void AddSqlParameters(ProductTransferObject product, SqlCommand command)
        {
            const string productNameParameter = "@productName";
            command.Parameters.Add(productNameParameter, SqlDbType.NVarChar, 40);
            command.Parameters[productNameParameter].Value = product.Name;

            const string supplierIdParameter = "@supplierId";
            command.Parameters.Add(supplierIdParameter, SqlDbType.Int);
            command.Parameters[supplierIdParameter].IsNullable = true;

            if (product.SupplierId != null)
            {
                command.Parameters[supplierIdParameter].Value = product.SupplierId;
            }
            else
            {
                command.Parameters[supplierIdParameter].Value = DBNull.Value;
            }

            const string categoryIdParameter = "@categoryId";
            command.Parameters.Add(categoryIdParameter, SqlDbType.Int);
            command.Parameters[categoryIdParameter].IsNullable = true;

            if (product.CategoryId != null)
            {
                command.Parameters[categoryIdParameter].Value = product.CategoryId;
            }
            else
            {
                command.Parameters[categoryIdParameter].Value = DBNull.Value;
            }

            const string quantityPerUnitParameter = "@quantityPerUnit";
            command.Parameters.Add(quantityPerUnitParameter, SqlDbType.NVarChar, 20);
            command.Parameters[quantityPerUnitParameter].IsNullable = true;

            if (product.QuantityPerUnit != null)
            {
                command.Parameters[quantityPerUnitParameter].Value = product.QuantityPerUnit;
            }
            else
            {
                command.Parameters[quantityPerUnitParameter].Value = DBNull.Value;
            }

            const string unitPriceParameter = "@unitPrice";
            command.Parameters.Add(unitPriceParameter, SqlDbType.Money);
            command.Parameters[unitPriceParameter].IsNullable = true;

            if (product.UnitPrice != null)
            {
                command.Parameters[unitPriceParameter].Value = product.UnitPrice;
            }
            else
            {
                command.Parameters[unitPriceParameter].Value = DBNull.Value;
            }

            const string unitsInStockParameter = "@unitsInStock";
            command.Parameters.Add(unitsInStockParameter, SqlDbType.SmallInt);
            command.Parameters[unitsInStockParameter].IsNullable = true;

            if (product.UnitsInStock != null)
            {
                command.Parameters[unitsInStockParameter].Value = product.UnitsInStock;
            }
            else
            {
                command.Parameters[unitsInStockParameter].Value = DBNull.Value;
            }

            const string unitsOnOrderParameter = "@unitsOnOrder";
            command.Parameters.Add(unitsOnOrderParameter, SqlDbType.SmallInt);
            command.Parameters[unitsOnOrderParameter].IsNullable = true;

            if (product.UnitsOnOrder != null)
            {
                command.Parameters[unitsOnOrderParameter].Value = product.UnitsOnOrder;
            }
            else
            {
                command.Parameters[unitsOnOrderParameter].Value = DBNull.Value;
            }

            const string reorderLevelParameter = "@reorderLevel";
            command.Parameters.Add(reorderLevelParameter, SqlDbType.SmallInt);
            command.Parameters[reorderLevelParameter].IsNullable = true;

            if (product.ReorderLevel != null)
            {
                command.Parameters[reorderLevelParameter].Value = product.ReorderLevel;
            }
            else
            {
                command.Parameters[reorderLevelParameter].Value = DBNull.Value;
            }

            const string discontinuedParameter = "@discontinued";
            command.Parameters.Add(discontinuedParameter, SqlDbType.Bit);
            command.Parameters[discontinuedParameter].Value = product.Discontinued;
        }

        private async Task<IList<ProductTransferObject>> ExecuteReaderAsync(SqlCommand command)
        {
            var products = new List<ProductTransferObject>();
            await this.connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await reader.ReadAsync())
            {
                products.Add(CreateProduct(reader));
            }

            return products;
        }
    }
}