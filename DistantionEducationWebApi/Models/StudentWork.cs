namespace DistantionEducationWebApi.Models
{
    public class StudentWork
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public IEnumerable<WorkTask> Tasks { get; set; } = new List<WorkTask>();
    }
}
