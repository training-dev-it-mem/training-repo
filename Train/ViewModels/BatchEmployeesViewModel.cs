using Train.Models.Identity;

namespace Train.ViewModels
{
    public class BatchEmployeesViewModel
    {
        public string BatchId { get; set; }

        public List<ApplicationUser> Employees { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
