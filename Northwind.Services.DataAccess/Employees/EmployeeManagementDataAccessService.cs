using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;

namespace Northwind.Services.DataAccess.Employees
{
    /// <summary>
    /// Provides an employee management service using data access. 
    /// </summary>
    public class EmployeeManagementDataAccessService : IEmployeeManagementService
    {
        private readonly IEmployeeDataAccessObject employeeDataAccessObject;

        /// <summary>
        /// Initializes a new instance of <see cref="EmployeeManagementDataAccessService"/> class.
        /// </summary>
        /// <param name="dataAccessFactory">A Northwind data acess factory.</param>
        public EmployeeManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            this.employeeDataAccessObject = dataAccessFactory.GetEmployeeDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<IList<Employee>> ShowEmployeesAsync(int offset, int limit)
        {
            return (await this.employeeDataAccessObject.SelectEmployeesAsync(offset, limit)).Select(GetEmployee).ToList();
        }

        /// <inheritdoc/>
        public bool TryShowEmployee(int employeeId, out Employee employee)
        {
            Task<EmployeeTransferObject> employeeTransferObjectTask;
            try
            {
                employeeTransferObjectTask = this.employeeDataAccessObject.FindEmployeeAsync(employeeId);
            }
            catch (EmployeeNotFoundException)
            {
                employee = null;
                return false;
            }

            if (employeeTransferObjectTask is null)
            {
                employee = null;
                return false;
            }
            
            employee = GetEmployee(employeeTransferObjectTask.Result);

            return true;
        }

        /// <inheritdoc/>
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            return await this.employeeDataAccessObject.InsertEmployeeAsync(GetEmployeeTransferObject(employee));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            return await this.employeeDataAccessObject.DeleteEmployeeAsync(employeeId);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            return await this.employeeDataAccessObject.UpdateEmployeeAsync(GetEmployeeTransferObject(employee));
        }

        private static Employee GetEmployee(EmployeeTransferObject employee) =>
            new()
            {
                Id = employee.Id,
                LastName = employee.LastName,
                FirstName = employee.FirstName,
                Title = employee.Title,
                TitleOfCourtesy = employee.TitleOfCourtesy,
                BirthDate = employee.BirthDate,
                HireDate = employee.HireDate,
                Address = employee.Address,
                City = employee.City,
                Region = employee.Region,
                PostalCode = employee.PostalCode,
                Country = employee.Country,
                HomePhone = employee.HomePhone,
                Extension = employee.Extension,
                Photo = employee.Photo,
                Notes = employee.Notes,
                ReportsTo = employee.ReportsTo,
                PhotoPath = employee.PhotoPath,
            };

        private static EmployeeTransferObject GetEmployeeTransferObject(Employee employee) =>
            new()
            {
                Id = employee.Id,
                LastName = employee.LastName,
                FirstName = employee.FirstName,
                Title = employee.Title,
                TitleOfCourtesy = employee.TitleOfCourtesy,
                BirthDate = employee.BirthDate,
                HireDate = employee.HireDate,
                Address = employee.Address,
                City = employee.City,
                Region = employee.Region,
                PostalCode = employee.PostalCode,
                Country = employee.Country,
                HomePhone = employee.HomePhone,
                Extension = employee.Extension,
                Photo = employee.Photo,
                Notes = employee.Notes,
                ReportsTo = employee.ReportsTo,
                PhotoPath = employee.PhotoPath,
            };
    }
}
