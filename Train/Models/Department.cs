namespace Train.Models
{
    public class Department
    {
        public Department()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
