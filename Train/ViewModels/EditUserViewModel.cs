using System.ComponentModel.DataAnnotations;

namespace Train.ViewModels
{
    public class EditUserViewModel
    {
        //To avoid NullReferenceExceptions at runtime,
        //initialise Claims and Roles with a new list in the constructor.
        public EditUserViewModel()
        {
            Roles = new List<string>();
        }
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public IList<string> Roles { get; set; }
    }
}
