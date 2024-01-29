using Train.Models;

namespace Train.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string Department { get; set; }
        public List<BatchEmployees> employees { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
