using Train.Models.Identity;

namespace Train.ViewModels
{
    public class AssignEmployeesViewModel
    {
        public string BatchId { get; set; }
        public List<ApplicationUser> Employees { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalCount { get; set; }
    }
}
