using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBeet.Data.Entities
{
    public class Suspicion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime TimeReported { get; set; }

        [Required]
        public string Reporter { get; set; }

        [Required]
        public string Suspect { get; set; }
    }
}
