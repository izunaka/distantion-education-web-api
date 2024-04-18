namespace DistantionEducationWebApi.Models
{
    public class WorkTask
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public TaskType Type { get; set; }

        public IEnumerable<TaskAnswer> Answers { get; set; } = new List<TaskAnswer>();
    }
}
