using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBeet.Data.Entities
{
    public class Keyword
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Label { get; set; }

        public string Regex { get; set; }

        public virtual ICollection<KeywordRecord> Records { get; set; }
    }
}
