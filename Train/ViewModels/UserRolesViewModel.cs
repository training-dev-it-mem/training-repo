using Train.Models;

namespace Train.ViewModels
{
    public class UserRolesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Department Department { get; set; }
        public List<string> Roles { get; set; }
    }
}
