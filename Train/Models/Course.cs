using Train.Enums;

namespace Train.Models
{
    public class Course 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public ProgramStatus Status{ get; set; }
        public DateTime AdattionDate { get; set; }
    }
}
