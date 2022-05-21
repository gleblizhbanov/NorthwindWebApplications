using System.Collections.Generic;

namespace NorthwindMvcClient.ViewModels
{
    public class EmployeesPageViewModel
    {
        public IEnumerable<EmployeeViewModel> Employees { get; set; }

        public PageViewModel PageModel { get; set; }
    }
}
