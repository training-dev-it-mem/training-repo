using Train.Models;

namespace Train.ViewModels
{
    public class CoursesViewModel
    {
        public List<Courses> Courses { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
