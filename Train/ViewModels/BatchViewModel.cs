using Train.Models;

namespace Train.ModelViews
{
    public class BatchViewModel
    {
        public List<Batch> batches { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
