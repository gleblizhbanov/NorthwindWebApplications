using System;

namespace NorthwindMvcClient.Models
{
    /// <summary>
    /// Represents an employee.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Gets or sets an employee identifier.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets an employee last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets an employee first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets an employee title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets an employee title of courtesy.
        /// </summary>
        public string TitleOfCourtesy { get; set; }

        /// <summary>
        /// Gets or sets an employee date of birth.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets an employee hire date.
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// Gets or sets an employee address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets an employee city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets an employee region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets an employee postal code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets an employee country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets an employee home phone.
        /// </summary>
        public string HomePhone { get; set; }

        /// <summary>
        /// Gets or sets an employee extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets an employee photo.
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// Gets or sets an employee notes.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets an employee that the employee reports to.
        /// </summary>
        public int? ReportsTo { get; set; }

        /// <summary>
        /// Gets or sets an employee photo path.
        /// </summary>
        public string PhotoPath { get; set; }
    }
}
