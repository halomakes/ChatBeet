using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Data.Entities
{
    /// <summary>
    /// Mapping between a string and its replacement
    /// </summary>
    public class ReplacementMapping
    {
        [JsonIgnore]
        public int SetId { get; set; }

        /// <summary>
        /// Input string to match
        /// </summary>
        [Required]
        public string Input { get; set; }

        /// <summary>
        /// Value to replace input with
        /// </summary>
        [Required]
        public string Replacement { get; set; }
    }
}
