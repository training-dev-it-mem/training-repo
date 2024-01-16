using System.ComponentModel.DataAnnotations.Schema;
using Train.Models.Identity;

namespace Train.Models
{
    public class BatchEmployees
    {
        public BatchEmployees()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string BatchId { get; set; }
        [ForeignKey("BatchId")]
        public Batch Batch { get; set; }
        public string EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public ApplicationUser Employee { get; set; }
    }
}
