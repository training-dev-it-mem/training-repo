using System.ComponentModel.DataAnnotations;

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
    }
}
