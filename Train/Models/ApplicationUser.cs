using Microsoft.AspNetCore.Identity;

namespace Train.Models.Identity

{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FullName { get; set; }
        public string ExtractUsernameFromEmail()
        {
            int atIndex = Email?.IndexOf('@') ?? -1;

            if (atIndex != -1)
            {
                return Email?[..atIndex] ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
