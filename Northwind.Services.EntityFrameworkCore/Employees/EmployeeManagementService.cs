using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;

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
        public async Task<IList<Employee>> ShowEmployeesAsync(int offset, int limit)
        {
            return await this.context.Employees.Skip(offset).Take(limit).ToListAsync();
        }

        /// <inheritdoc/>
        public bool TryShowEmployee(int employeeId, out Employee employee)
        {
            employee = this.context.Find<Employee>(employeeId);
            return employee is not null;
        }

        /// <inheritdoc/>
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            var entityEntry = await this.context.AddAsync(employee);
            await this.context.SaveChangesAsync();
            return entityEntry.Entity.Id;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            var employee = await this.context.FindAsync<Employee>(employeeId);
            this.context.Remove(employee);
            return await this.context.SaveChangesAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            var oldEmployee = await this.context.FindAsync<Employee>(employeeId);
            if (oldEmployee is null)
            {
                return false;
            }

            this.context.Entry(oldEmployee).CurrentValues.SetValues(employee);
            this.context.Update(oldEmployee);
            return await this.context.SaveChangesAsync() > 0;
        }
    }
}
