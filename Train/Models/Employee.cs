using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Train.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public Managers Manager { get; set; }
    }
}
