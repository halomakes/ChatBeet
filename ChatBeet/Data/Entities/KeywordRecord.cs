using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBeet.Data.Entities
{
    public class KeywordRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int KeywordId { get; set; }
        public string Nick { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        [ForeignKey("KeywordId")]
        public virtual Keyword Keyword { get; set; }
    }
}
