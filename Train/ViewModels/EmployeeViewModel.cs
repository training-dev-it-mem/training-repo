using Train.Models;

namespace Train.ModelViews
{
    public class EmployeeViewModel
    {
        public List<Employee> Employees { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
