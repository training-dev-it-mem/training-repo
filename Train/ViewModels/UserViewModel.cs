using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Train.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }

        [Display(Name = "Roles")]
        public string[] Roles { get; set; }

        public List<SelectListItem>? DepartmentList { get; set; }
        public List<SelectListItem>? RolesList { get; set; }
    }
}
