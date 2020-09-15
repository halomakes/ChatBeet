using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBeet.Data.Entities
{
    public class ReplacementSet
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Command { get; set; }

        public virtual ICollection<ReplacementMapping> Mappings { get; set; }
    }
}
