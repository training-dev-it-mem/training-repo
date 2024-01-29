using System.ComponentModel.DataAnnotations;
using Train.Enums;
using Train.Models;
using Train.Models.Identity;

namespace Train.ViewModels
{
    public class CourseViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please enter course name")]
        [RegularExpression(@"^[a-zA-Z0-9\s'.-]{1,25}$", ErrorMessage = "Only letters, numbers, and whitespace allowed with a maximum of 25 characters")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter course description")]
        [RegularExpression(@"^[a-zA-Z0-9\s'.-]{1,150}$", ErrorMessage = "Only letters, numbers, and whitespace allowed with a maximum of 150 characters")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter location")]
        [RegularExpression(@"^[a-zA-Z0-9\s'.-]{1,25}$", ErrorMessage = "Only letters, numbers, and whitespace allowed with a maximum of 25 characters")]
        public string Location { get; set; }
        public DateTime AdattionDate { get; set; } = DateTime.Now;
        public int Seats { get; set; }
    }
}
