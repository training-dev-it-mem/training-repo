using System.ComponentModel.DataAnnotations;
using Train.Enums;
using Train.Models.Identity;

namespace Train.Models
{
    public class Course 
    {
        public Course()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public ProgramStatus Status{ get; set; }
        public DateTime AdattionDate { get; set; }
        public int Seats { get; set; }
        public ICollection<Batch> Batches { get; set; }
        public string UserId { get; internal set; }
    }
}
