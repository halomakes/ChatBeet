using System.ComponentModel.DataAnnotations;

namespace DtellaRules.Data.Entities
{
    public class MemoryCell
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }

        public string Author { get; set; }
    }
}
