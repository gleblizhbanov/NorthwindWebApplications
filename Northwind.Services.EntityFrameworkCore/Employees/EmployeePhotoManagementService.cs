using System.IO;
using System.Threading.Tasks;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.EntityFrameworkCore.Entities;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    /// <summary>
    /// Represents a stub for an employee photo management service.
    /// </summary>
    public class EmployeePhotoManagementService : IEmployeePhotoManagementService
    {
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePhotoManagementService"/> class.
        /// </summary>
        /// <param name="context">A db content.</param>
        public EmployeePhotoManagementService(NorthwindContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public bool TryShowPhoto(int employeeId, out byte[] bytes)
        {
            var employee = this.context.Find<Employee>(employeeId);
            if (employee is null)
            {
                bytes = null;
                return false;
            }

            bytes = employee.Photo;
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePhotoAsync(int employeeId, Stream stream)
        {
            var employee = await this.context.FindAsync<Employee>(employeeId).ConfigureAwait(false);
            if (employee is null)
            {
                return false;
            }

            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            employee.Photo = memoryStream.ToArray();
            this.context.Update(employee);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPhotoAsync(int employeeId)
        {
            var employee = await this.context.FindAsync<Employee>(employeeId).ConfigureAwait(false);
            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;
            this.context.Update(employee);
            return await this.context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }
    }
}
