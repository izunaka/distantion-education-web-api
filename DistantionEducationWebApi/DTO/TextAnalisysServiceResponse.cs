namespace DistantionEducationWebApi.DTO
{
    public class TextAnalisysServiceResponse
    {
        public double Similarity { get; set; }

        public IEnumerable<string> GeneralTerminsInFirstText { get; set; }

        public IEnumerable<string> GeneralTerminsInSecondText { get; set; }

        public IEnumerable<string> ExtraTerminsInFirstText { get; set; }

        public IEnumerable<string> ExtraTerminsInSecondText { get; set; }
    }
}
