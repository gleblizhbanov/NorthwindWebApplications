using System.ComponentModel;
using NorthwindMvcClient.Models;

namespace NorthwindMvcClient.ViewModels
{
    /// <summary>
    /// Represents an employee.
    /// </summary>
    public class EmployeeViewModel
    {
        public EmployeeViewModel()
        {
        }

        public EmployeeViewModel(Employee employee)
        {
            this.Id = employee.EmployeeId;
            this.FirstName = employee.FirstName;
            this.LastName = employee.LastName;
            this.Title = employee.Title;
            this.Photo = employee.Photo;
        }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [DisplayName("ID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the employee first name.
        /// </summary>
        [DisplayName("First name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the employee last name.
        /// </summary>
        [DisplayName("Last name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the employee title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the employee photo.
        /// </summary>
        public byte[] Photo { get; set; }
    }
}
