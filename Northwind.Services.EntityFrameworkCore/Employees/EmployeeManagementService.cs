using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.EntityFrameworkCore.Entities;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    /// <summary>
    /// Represents a stub for a employee management service.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="context">A db context.</param>
        public EmployeeManagementService(NorthwindContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<IList<Models.Employee>> ShowEmployeesAsync(int offset, int limit)
        {
            return limit != int.MaxValue
                ? this.context.Employees.Skip(offset).Take(limit).AsEnumerable().Select(GetEmployeeModel).ToList()
                : this.context.Employees.Skip(offset).AsEnumerable().Select(GetEmployeeModel).ToList();
        }

        /// <inheritdoc/>
        public bool TryShowEmployee(int employeeId, out Models.Employee employee)
        {
            var employeeEntity = this.context.Find<Employee>(employeeId);
            if (employeeEntity is null)
            {
                employee = null;
                return false;
            }

            employee = GetEmployeeModel(employeeEntity);
            return true;
        }

        /// <inheritdoc/>
        public async Task<int> CreateEmployeeAsync(Models.Employee employee)
        {
            var entityEntry = await this.context.AddAsync(GetEmployeeEntity(employee)).ConfigureAwait(false);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return entityEntry.Entity.EmployeeId;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            var employee = await this.context.FindAsync<Employee>(employeeId).ConfigureAwait(false);
            this.context.Remove(employee);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Models.Employee employee)
        {
            var oldEmployee = await this.context.FindAsync<Employee>(employeeId).ConfigureAwait(false);
            if (oldEmployee is null)
            {
                return false;
            }

            oldEmployee.EmployeeId = employee.EmployeeId;
            oldEmployee.LastName = employee.LastName;
            oldEmployee.FirstName = employee.FirstName;
            oldEmployee.Title = employee.Title;
            oldEmployee.TitleOfCourtesy = employee.TitleOfCourtesy;
            oldEmployee.BirthDate = employee.BirthDate;
            oldEmployee.HireDate = employee.HireDate;
            oldEmployee.Address = employee.Address;
            oldEmployee.City = employee.City;
            oldEmployee.Region = employee.Region;
            oldEmployee.PostalCode = employee.PostalCode;
            oldEmployee.Country = employee.Country;
            oldEmployee.HomePhone = employee.HomePhone;
            oldEmployee.Extension = employee.Extension;
            oldEmployee.Photo = employee.Photo;
            oldEmployee.Notes = employee.Notes;
            oldEmployee.ReportsTo = employee.ReportsTo;
            oldEmployee.PhotoPath = employee.PhotoPath;
            this.context.Update(oldEmployee);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        private static Models.Employee GetEmployeeModel(Employee employee) =>
            new ()
            {
                EmployeeId = employee.EmployeeId,
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

        private static Employee GetEmployeeEntity(Models.Employee employee) =>
            new ()
            {
                EmployeeId = employee.EmployeeId,
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
