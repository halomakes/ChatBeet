using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Data.Entities
{
    public class ReplacementMapping
    {
        [JsonIgnore]
        public int SetId { get; set; }

        [Required]
        public string Input { get; set; }

        [Required]
        public string Replacement { get; set; }
    }
}
