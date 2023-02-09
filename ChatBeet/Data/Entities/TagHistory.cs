using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBeet.Data.Entities;

public class TagHistory
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Nick { get; set; }
    public string Tag { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}