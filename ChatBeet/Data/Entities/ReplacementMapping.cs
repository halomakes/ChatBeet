using Newtonsoft.Json;

namespace ChatBeet.Data.Entities
{
    public class ReplacementMapping
    {
        [JsonIgnore]
        public int SetId { get; set; }

        public string Input { get; set; }

        public string Replacement { get; set; }
    }
}
