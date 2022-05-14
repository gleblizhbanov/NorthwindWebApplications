using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Northwind.DataAccess.Employees;

namespace Northwind.DataAccess.SqlServer.Employees
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class EmployeeSqlServerDataAccessObject : IEmployeeDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public EmployeeSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertEmployeeAsync(EmployeeTransferObject employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await using var sqlCommand = new SqlCommand("InsertEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(sqlCommand, employee);

            await this.connection.OpenAsync().ConfigureAwait(false);
            var id = await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);
            await this.connection.CloseAsync().ConfigureAwait(false);

            return (int)id;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            await using var sqlCommand = new SqlCommand("DeleteEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string employeeIdParameter = "@employeeId";
            sqlCommand.Parameters.Add(employeeIdParameter, SqlDbType.Int);
            sqlCommand.Parameters[employeeIdParameter].Value = employeeId;

            await this.connection.OpenAsync().ConfigureAwait(false);
            var result = await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);
            await this.connection.CloseAsync().ConfigureAwait(false);

            return (int)result > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(EmployeeTransferObject employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await using var sqlCommand = new SqlCommand("UpdateEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string employeeIdParameter = "@employeeId";
            sqlCommand.Parameters.Add(employeeIdParameter, SqlDbType.Int);
            sqlCommand.Parameters[employeeIdParameter].Value = employee.EmployeeId;

            AddSqlParameters(sqlCommand, employee);

            await this.connection.OpenAsync().ConfigureAwait(false);
            var result = await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);
            await this.connection.CloseAsync().ConfigureAwait(false);

            return (int)result > 0;
        }

        /// <inheritdoc/>
        public async Task<EmployeeTransferObject> FindEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            await using var sqlCommand = new SqlCommand("FindEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string employeeIdParameter = "@employeeId";
            sqlCommand.Parameters.Add(employeeIdParameter, SqlDbType.Int);
            sqlCommand.Parameters[employeeIdParameter].Value = employeeId;

            await this.connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false);

            if (!await reader.ReadAsync().ConfigureAwait(false))
            {
                throw new EmployeeNotFoundException(employeeId);
            }

            var employee = BuildEmployee(reader);

            return employee;
        }

        /// <inheritdoc/>
        public async Task<IList<EmployeeTransferObject>> SelectEmployeesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await using var sqlCommand = new SqlCommand("SelectEmployees", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string offsetParameter = "@offset";
            sqlCommand.Parameters.Add(offsetParameter, SqlDbType.Int);
            sqlCommand.Parameters[offsetParameter].Value = offset;

            const string limitParameter = "@limit";
            sqlCommand.Parameters.Add(limitParameter, SqlDbType.Int);
            sqlCommand.Parameters[limitParameter].Value = limit;

            var employees = new List<EmployeeTransferObject>();
            await this.connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                employees.Add(BuildEmployee(reader));
            }

            return employees;
        }

        private static void AddSqlParameters(SqlCommand command, EmployeeTransferObject employee)
        {
            const string lastNameParameter = "@lastName";
            command.Parameters.Add(lastNameParameter, SqlDbType.NVarChar, 20);
            command.Parameters[lastNameParameter].Value = (object)employee.LastName ?? DBNull.Value;

            const string firstNameParameter = "@firstName";
            command.Parameters.Add(firstNameParameter, SqlDbType.NVarChar, 10);
            command.Parameters[firstNameParameter].Value = (object)employee.FirstName ?? DBNull.Value;

            const string titleParameter = "@title";
            command.Parameters.Add(titleParameter, SqlDbType.NVarChar, 30);
            command.Parameters[titleParameter].IsNullable = true;
            command.Parameters[titleParameter].Value = (object)employee.Title ?? DBNull.Value;

            const string titleOfCourtesyParameter = "@titleOfCourtesy";
            command.Parameters.Add(titleOfCourtesyParameter, SqlDbType.NVarChar, 25);
            command.Parameters[titleOfCourtesyParameter].IsNullable = true;
            command.Parameters[titleOfCourtesyParameter].Value = (object)employee.TitleOfCourtesy ?? DBNull.Value;

            const string birthDateParameter = "@birthDate";
            command.Parameters.Add(birthDateParameter, SqlDbType.DateTime);
            command.Parameters[birthDateParameter].IsNullable = true;
            command.Parameters[birthDateParameter].Value = (object)employee.BirthDate ?? DBNull.Value;

            const string hireDateParameter = "@hireDate";
            command.Parameters.Add(hireDateParameter, SqlDbType.DateTime);
            command.Parameters[hireDateParameter].IsNullable = true;
            command.Parameters[hireDateParameter].Value = (object)employee.HireDate ?? DBNull.Value;

            const string addressParameter = "@address";
            command.Parameters.Add(addressParameter, SqlDbType.NVarChar, 60);
            command.Parameters[addressParameter].IsNullable = true;
            command.Parameters[addressParameter].Value = (object)employee.Address ?? DBNull.Value;

            const string cityParameter = "@city";
            command.Parameters.Add(cityParameter, SqlDbType.NVarChar, 15);
            command.Parameters[cityParameter].IsNullable = true;
            command.Parameters[cityParameter].Value = (object)employee.City ?? DBNull.Value;

            const string regionParameter = "@region";
            command.Parameters.Add(regionParameter, SqlDbType.NVarChar, 15);
            command.Parameters[regionParameter].IsNullable = true;
            command.Parameters[regionParameter].Value = (object)employee.Region ?? DBNull.Value;

            const string postalCodeParameter = "@postalCode";
            command.Parameters.Add(postalCodeParameter, SqlDbType.NVarChar, 10);
            command.Parameters[postalCodeParameter].IsNullable = true;
            command.Parameters[postalCodeParameter].Value = (object)employee.PostalCode ?? DBNull.Value;

            const string countryParameter = "@country";
            command.Parameters.Add(countryParameter, SqlDbType.NVarChar, 15);
            command.Parameters[countryParameter].IsNullable = true;
            command.Parameters[countryParameter].Value = (object)employee.Country ?? DBNull.Value;

            const string homePhoneParameter = "@homePhone";
            command.Parameters.Add(homePhoneParameter, SqlDbType.NVarChar, 24);
            command.Parameters[homePhoneParameter].IsNullable = true;
            command.Parameters[homePhoneParameter].Value = (object)employee.HomePhone ?? DBNull.Value;

            const string extensionParameter = "@extension";
            command.Parameters.Add(extensionParameter, SqlDbType.NVarChar, 4);
            command.Parameters[extensionParameter].IsNullable = true;
            command.Parameters[extensionParameter].Value = (object)employee.Extension ?? DBNull.Value;

            const string photoParameter = "@photo";
            command.Parameters.Add(photoParameter, SqlDbType.Image);
            command.Parameters[photoParameter].IsNullable = true;
            command.Parameters[photoParameter].Value = (object)employee.Photo ?? DBNull.Value;

            const string notesParameter = "@notes";
            command.Parameters.Add(notesParameter, SqlDbType.NText);
            command.Parameters[notesParameter].IsNullable = true;
            command.Parameters[notesParameter].Value = (object)employee.Notes ?? DBNull.Value;

            const string reportsToParameter = "@reportsTo";
            command.Parameters.Add(reportsToParameter, SqlDbType.Int);
            command.Parameters[reportsToParameter].IsNullable = true;
            command.Parameters[reportsToParameter].Value = (object)employee.ReportsTo ?? DBNull.Value;

            const string photoPathParameter = "@photoPath";
            command.Parameters.Add(photoPathParameter, SqlDbType.NVarChar, 255);
            command.Parameters[photoPathParameter].IsNullable = true;
            command.Parameters[photoPathParameter].Value = (object)employee.PhotoPath ?? DBNull.Value;
        }

        private static EmployeeTransferObject BuildEmployee(SqlDataReader reader)
        {
            return new EmployeeTransferObject()
            {
                EmployeeId = (int)reader["EmployeeId"],
                LastName = (string)reader["LastName"],
                FirstName = (string)reader["FirstName"],
                Title = (string)(reader["Title"] != DBNull.Value ? reader["Title"] : null),
                TitleOfCourtesy = (string)(reader["TitleOfCourtesy"] != DBNull.Value ? reader["TitleOfCourtesy"] : null),
                BirthDate = (DateTime?)(reader["BirthDate"] != DBNull.Value ? reader["BirthDate"] : null),
                HireDate = (DateTime?)(reader["HireDate"] != DBNull.Value ? reader["HireDate"] : null),
                Address = (string)(reader["Address"] != DBNull.Value ? reader["Address"] : null),
                City = (string)(reader["City"] != DBNull.Value ? reader["City"] : null),
                Region = (string)(reader["Title"] != DBNull.Value ? reader["Title"] : null),
                PostalCode = (string)(reader["PostalCode"] != DBNull.Value ? reader["PostalCode"] : null),
                Country = (string)(reader["Country"] != DBNull.Value ? reader["Country"] : null),
                HomePhone = (string)(reader["HomePhone"] != DBNull.Value ? reader["HomePhone"] : null),
                Extension = (string)(reader["Extension"] != DBNull.Value ? reader["Extension"] : null),
                Photo = (byte[])(reader["Photo"] != DBNull.Value ? reader["Photo"] : null),
                Notes = (string)(reader["Notes"] != DBNull.Value ? reader["Notes"] : null),
                ReportsTo = (int?)(reader["ReportsTo"] != DBNull.Value ? reader["ReportsTo"] : null),
                PhotoPath = (string)(reader["PhotoPath"] != DBNull.Value ? reader["PhotoPath"] : null),
            };
        }
    }
}