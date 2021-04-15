using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Data.Entities
{
    /// <summary>
    /// Stores a definition
    /// </summary>
    public class MemoryCell
    {
        /// <summary>
        /// Key of definition
        /// </summary>
        [Key]
        public string Key { get; set; }

        /// <summary>
        /// Value of definition
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Person who set definition
        /// </summary>
        public string Author { get; set; }
    }
}
