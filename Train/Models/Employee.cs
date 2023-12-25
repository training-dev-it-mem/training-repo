using System.ComponentModel.DataAnnotations;

namespace Train.Models
{
    public class Employee
    {
        [Key]
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
