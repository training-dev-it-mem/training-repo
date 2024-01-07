using System.ComponentModel.DataAnnotations;
using Train.Models;

namespace Train.ViewModels
{
    public class BatchViewModel
    {
        
        public int Id { get; set; }
        [DisplayFormat]
        [DataType(DataType.Date)]
        [Required]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Please select Course")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }
    }
}
