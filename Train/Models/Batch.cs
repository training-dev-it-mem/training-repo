using System.ComponentModel.DataAnnotations;

namespace Train.Models
{
    public class Batch
    {
        [Key]
        public int number { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public TimeSpan StarTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
