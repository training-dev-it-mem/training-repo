using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Train.Models
{
    public class Batch
    {
        public Guid Id { get; set; }
        [DisplayFormat]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public Guid CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
