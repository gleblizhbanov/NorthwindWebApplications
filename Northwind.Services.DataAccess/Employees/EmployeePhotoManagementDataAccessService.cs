using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;

namespace Northwind.Services.DataAccess.Employees
{
    /// <summary>
    /// Provides a management service using data access object for employee photos.
    /// </summary>
    public class EmployeePhotoManagementDataAccessService : IEmployeePhotoManagementService
    {
        private readonly IEmployeeDataAccessObject employeeDataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePhotoManagementDataAccessService"/> class.
        /// </summary>
        /// <param name="dataAccessFactory">A Northwind data access factory.</param>
        public EmployeePhotoManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            this.employeeDataAccessObject = dataAccessFactory.GetEmployeeDataAccessObject();
        }

        /// <inheritdoc/>
        public bool TryShowPhoto(int employeeId, out byte[] bytes)
        {
            var employeeTask = this.employeeDataAccessObject.FindEmployeeAsync(employeeId);
            try
            {
                bytes = employeeTask.Result.Photo;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(e => e.GetType() == typeof(EmployeeNotFoundException)))
                {
                    bytes = null;
                    return false;
                }

                throw;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePhotoAsync(int employeeId, Stream stream)
        {
            EmployeeTransferObject employee;
            try
            {
                employee = await this.employeeDataAccessObject.FindEmployeeAsync(employeeId).ConfigureAwait(false);
            }
            catch (EmployeeNotFoundException)
            {
                return false;
            }

            if (employee is null)
            {
                return false;
            }

            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            employee.Photo = memoryStream.ToArray();
            return await this.employeeDataAccessObject.UpdateEmployeeAsync(employee).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPhotoAsync(int employeeId)
        {
            EmployeeTransferObject employee;
            try
            {
                employee = await this.employeeDataAccessObject.FindEmployeeAsync(employeeId).ConfigureAwait(false);
            }
            catch (EmployeeNotFoundException)
            {
                return false;
            }

            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;

            return true;
        }
    }
}
