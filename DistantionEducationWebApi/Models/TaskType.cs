namespace DistantionEducationWebApi.Models
{
    public class TaskType
    {
        public int? Id { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }

        public TypeRule? Rule { get; set; }
    }
}
