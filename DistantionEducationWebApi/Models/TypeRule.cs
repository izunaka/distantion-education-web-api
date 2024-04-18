using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace DistantionEducationWebApi.Models
{
    public class TypeRule
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RuleContol Contol { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RuleAnswer Answer { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Rule Rule { get; set; }
    }

    public enum RuleType
    {
        Single = 0,
        Multi = 1
    }

    public enum RuleContol
    {
        Selection = 0,
        MultiSelection = 1,
        Input = 2,
        TextArea = 3
    }

    public enum RuleAnswer
    {
        Single = 0,
        Multi = 1
    }

    public enum Rule
    {
        Match = 0,
        String = 1,
        Text = 2
    }
}
