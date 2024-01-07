using Microsoft.AspNetCore.Identity;

namespace Train.Models.Identity

{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FullName { get; set; }

    }
}
