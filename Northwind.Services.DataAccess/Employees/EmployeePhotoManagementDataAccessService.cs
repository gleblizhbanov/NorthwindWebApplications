using System.IO;
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
        /// Initializes a new instance of <see cref="EmployeePhotoManagementDataAccessService"/> class.
        /// </summary>
        /// <param name="dataAccessFactory">A Northwind data access factory.</param>
        public EmployeePhotoManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            this.employeeDataAccessObject = dataAccessFactory.GetEmployeeDataAccessObject();
        }

        /// <inheritdoc/>
        public bool TryShowPhoto(int employeeId, out byte[] bytes)
        {
            Task<EmployeeTransferObject> employeeTask;
            try
            {
                employeeTask = this.employeeDataAccessObject.FindEmployeeAsync(employeeId);
            }
            catch (EmployeeNotFoundException)
            {
                bytes = null;
                return false;
            }

            if (employeeTask is null)
            {
                bytes = null;
                return false;
            }
            
            bytes = employeeTask.Result.Photo;
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePhotoAsync(int employeeId, Stream stream)
        {
            EmployeeTransferObject employee;
            try
            {
                employee = await this.employeeDataAccessObject.FindEmployeeAsync(employeeId);
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
            await stream.CopyToAsync(memoryStream);
            employee.Photo = memoryStream.ToArray();
            return await this.employeeDataAccessObject.UpdateEmployeeAsync(employee);
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPhotoAsync(int employeeId)
        {
            EmployeeTransferObject employee;
            try
            {
                employee = await this.employeeDataAccessObject.FindEmployeeAsync(employeeId);
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
