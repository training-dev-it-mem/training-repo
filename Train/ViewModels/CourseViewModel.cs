using Train.Models;

namespace Train.ModelViews
{
    public class CourseViewModel
    {
        public List<Course> Courses { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
