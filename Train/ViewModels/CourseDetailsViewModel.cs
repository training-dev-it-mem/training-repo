using Train.Enums;
using Train.Models;

namespace Train.ViewModels
{
    public class CourseDetailsViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public ProgramStatus Status { get; set; }
        public DateTime AdattionDate { get; set; }
        public int Seats { get; set; }
        public string CreatedBy { get; set; }
        public List<Batch> batches { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
