using System.ComponentModel.DataAnnotations.Schema;
using Train.Models.Identity;

namespace Train.Models
{
    public class BatchEmployees
    {
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        [ForeignKey("BatchId")]
        public Batch Batch { get; set; }
        public string EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public ApplicationUser Employee { get; set; }
    }
}
