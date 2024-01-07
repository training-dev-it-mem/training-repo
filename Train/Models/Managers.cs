using System.ComponentModel.DataAnnotations;

namespace Train.Models
{
    public class Managers
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the name")]
        [MinLength(7, ErrorMessage = "The field must be at least 7 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s'.-]{10,40}$", ErrorMessage = "only alphabetic allowed")]
        public string Name { get; set; }

        [MinLength(7, ErrorMessage = "The field must be at least 7 characters long.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
