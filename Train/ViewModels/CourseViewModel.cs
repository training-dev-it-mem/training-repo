using System.ComponentModel.DataAnnotations;
using Train.Enums;
using Train.Models;

namespace Train.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the name")]
        [MinLength(5, ErrorMessage = "The field must be at least 7 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s'.-]{5,40}$", ErrorMessage = "only alphabetic allowed")]
        public string Name { get; set; }

        [RegularExpression(@"^[a-zA-Z\s'.-]{5,100}$", ErrorMessage = "only alphabetic allowed")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the description")]
        [MinLength(5, ErrorMessage = "The field must be at least 5 characters long.")]
        public string Description { get; set; }
        public string Location { get; set; }

        [Required(ErrorMessage = "Please select a status")]
        public ProgramStatus Status { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the date")]
        public DateTime AdattionDate { get; set; }
        public int Seats { get; set; }
        public int UserId { get; set; }
    }
}
