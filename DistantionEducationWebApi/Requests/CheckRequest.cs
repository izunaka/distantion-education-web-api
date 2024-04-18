namespace DistantionEducationWebApi.Requests
{
    public class CheckRequest
    {
        public List<CheckAnswer> Answers { get; set; } = new List<CheckAnswer>();

        public string? CurrentMethod { get; set; }

        public bool? UseFreequency {  get; set; }

        public bool? UseSynonyms { get; set; }

        public double? SynonymsMaxFine {  get; set; }
    }

    public class CheckAnswer
    {
        public Guid TaskId { get; set; }

        public List<AnswerValue> Values { get; set; } = new List<AnswerValue>();
    }

    public class AnswerValue
    {
        public Guid ValueId { get; set; }

        public string? ValueDescription { get; set; }
    }
}
