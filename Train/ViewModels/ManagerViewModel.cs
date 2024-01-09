using System.ComponentModel.DataAnnotations;
using Train.Enums;
using Train.Models;

namespace Train.ViewModels
{
    public class ManagerViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the name")]
        [MinLength(5, ErrorMessage = "The field must be at least 5 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s'.-]{5,40}$", ErrorMessage = "only alphabetic allowed")]
        public string Name { get; set; }

        [MinLength(7, ErrorMessage = "The field must be at least 7 characters long.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public Department Department { get; set; }

     
    }
}
