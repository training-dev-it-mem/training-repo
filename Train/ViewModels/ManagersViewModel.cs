using Train.Models;

namespace Train.ViewModels
{
    public class ManagersViewModel
    {
        public List<Managers> Managers { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
