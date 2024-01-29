using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Train.Models.Identity

{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public string DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
