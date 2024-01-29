using System.ComponentModel.DataAnnotations;
using Train.Models.Identity;

namespace Train.ViewModels
{
    public class UsersViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }
    }
}
