using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Models;
using Northwind.Services.Employees;

namespace NorthwindApiApp.Controllers
{
    /// <summary>
    /// Provides a controller to work with employees.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeManagementService employeeManagementService;
        private readonly IEmployeePhotoManagementService employeePhotoManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesController"/> class.
        /// </summary>
        /// <param name="employeeManagementService">An employee management service.</param>
        /// <param name="employeePhotoManagementService">An employee photo management service.</param>
        public EmployeesController(IEmployeeManagementService employeeManagementService, IEmployeePhotoManagementService employeePhotoManagementService)
        {
            this.employeeManagementService = employeeManagementService;
            this.employeePhotoManagementService = employeePhotoManagementService;
        }

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="employee">An employee to add.</param>
        /// <returns>An action result.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEmployeeAsync(Employee employee)
        {
            int id = await this.employeeManagementService.CreateEmployeeAsync(employee).ConfigureAwait(false);
            if (id > 0)
            {
                return this.CreatedAtAction("CreateEmployee", new { id }, employee);
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Gets all employees.
        /// </summary>
        /// <returns>All employees.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesAsync()
        {
            if (await this.employeeManagementService.ShowEmployeesAsync(0, int.MaxValue).ConfigureAwait(false) is not List<Employee> employees)
            {
                return this.BadRequest();
            }

            return employees;
        }

        /// <summary>
        /// Gets an employee by id.
        /// </summary>
        /// <param name="id">An employee id.</param>
        /// <returns>An employee with given id.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Employee> GetEmployee(int id)
        {
            if (this.employeeManagementService.TryShowEmployee(id, out var employee))
            {
                return employee;
            }

            return this.NotFound();
        }

        /// <summary>
        /// Updates an employee with specific id.
        /// </summary>
        /// <param name="id">An employee id.</param>
        /// <param name="employee">A new employee.</param>
        /// <returns>An action result.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEmployeeAsync(int id, Employee employee)
        {
            if (employee is null || id != employee.EmployeeId)
            {
                return this.BadRequest();
            }

            if (await this.employeeManagementService.UpdateEmployeeAsync(id, employee).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Deletes an employee with specific id.
        /// </summary>
        /// <param name="id">An employee id.</param>
        /// <returns>An action result.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEmployeeAsync(int id)
        {
            if (await this.employeeManagementService.DestroyEmployeeAsync(id).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Gets the employee photo.
        /// </summary>
        /// <param name="id">An employee identifier.</param>
        /// <returns>An employee photo.</returns>
        [HttpGet("{id:int}/photo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<byte[]> GetPhoto(int id)
        {
            if (this.employeePhotoManagementService.TryShowPhoto(id, out var bytes))
            {
                return this.File(bytes[78..], "image/jpg");
            }

            if (!this.employeeManagementService.TryShowEmployee(id, out _))
            {
                return this.NotFound();
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Uploads an employee photo.
        /// </summary>
        /// <param name="id">An employee identifier.</param>
        /// <param name="file">A picture file.</param>
        /// <returns>An action result.</returns>
        [HttpPut("{id:int}/photo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadPhotoAsync(int id, IFormFile file)
        {
            if (file is null || !await this.employeePhotoManagementService.UpdatePhotoAsync(id, file.OpenReadStream()).ConfigureAwait(false))
            {
                return this.BadRequest();
            }

            return this.NoContent();
        }

        /// <summary>
        /// Deletes the employee photo.
        /// </summary>
        /// <param name="id">A product category identifier.</param>
        /// <returns>An action result.</returns>
        [HttpDelete("{id:int}/photo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePhotoAsync(int id)
        {
            if (await this.employeePhotoManagementService.DestroyPhotoAsync(id).ConfigureAwait(false))
            {
                return this.NoContent();
            }

            if (!this.employeeManagementService.TryShowEmployee(id, out _))
            {
                return this.NotFound();
            }

            return this.BadRequest();
        }
    }
}
