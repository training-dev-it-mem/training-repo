using Train.Models;

namespace Train.ViewModels
{
    public class BatchsViewModel
    {
        public List<Batch> batches { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
