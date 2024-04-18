namespace DistantionEducationWebApi.Models
{
    public class TextAnalisysCustomParams
    {
        public TextAnalisysMethod? Method { get; set; }

        public bool? UseSynonyms { get; set; }

        public double? SynonymsMaxFine { get; set; }

        public bool? UseFrequency {  get; set; }
    }

    public enum TextAnalisysMethod
    {
        Jaccard = 0,
        Tfidf = 1,
        Bert = 2
    }
}
