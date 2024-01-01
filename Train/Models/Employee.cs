using System.ComponentModel.DataAnnotations;

namespace Train.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the name")]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
