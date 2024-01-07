using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Train.Models
{
    public class Batch
    {
      
        public int Id { get; set; }
        [DisplayFormat]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Courses Course { get; set; }
    }
}
