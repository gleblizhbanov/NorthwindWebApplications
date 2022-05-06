using System.IO;
using System.Threading.Tasks;

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents a managements service for product category pictures.
    /// </summary>
    public interface IEmployeePhotoManagementService
    {
        /// <summary>
        /// Try to show an employee photo.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <param name="bytes">An array of photo bytes.</param>
        /// <returns>True if an employee exists; otherwise false.</returns>
        bool TryShowPhoto(int employeeId, out byte[] bytes);

        /// <summary>
        /// Update an employee photo.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <returns>True if an employee exists; otherwise false.</returns>
        Task<bool> UpdatePhotoAsync(int employeeId, Stream stream);

        /// <summary>
        /// Destroy an employee photo.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <returns>True if an employee exists; otherwise false.</returns>
        Task<bool> DestroyPhotoAsync(int employeeId);
    }
}
