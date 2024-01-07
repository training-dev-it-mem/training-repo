using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Train.Models;

namespace Train.ViewModels
{
    public class EmployeeViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the name")]
        [MinLength(7, ErrorMessage = "The field must be at least 7 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s'.-]{10,40}$", ErrorMessage = "only alphabetic allowed")]
        public string Name { get; set; }

        [MinLength(7, ErrorMessage = "The field must be at least 7 characters long.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select a manager")]
        [Display(Name = "Manager")]
        public int ManagerId { get; set; }
    }
}
