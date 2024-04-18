using DistantionEducationWebApi.Models;

namespace DistantionEducationWebApi.Responses
{
    public class CheckResponse
    {
        public IEnumerable<TaskResult> Tasks { get; set; } = new List<TaskResult>();

        public double TotalScore;
    }

    public class TaskResult
    {
        public Guid TaskId {  get; set; }

        public double Score { get; set; }

        public IEnumerable<TaskAnswer> RightAnswers { get; set; } = new List<TaskAnswer>();

        public IEnumerable<string> GeneralTerminsInFirstText { get; set; }

        public IEnumerable<string> GeneralTerminsInSecondText { get; set; }

        public IEnumerable<string> ExtraTerminsInFirstText { get; set; }

        public IEnumerable<string> ExtraTerminsInSecondText { get; set; }
    }
}
