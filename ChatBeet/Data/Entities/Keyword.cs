using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ChatBeet.Data.Entities
{
    public class Keyword
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Label { get; set; }

        public string Regex { get; set; }

        public int SortOrder { get; set; }

        [JsonIgnore]
        public virtual ICollection<KeywordRecord> Records { get; set; }
    }
}
