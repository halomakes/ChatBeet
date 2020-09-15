using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBeet.Data.Entities
{
    /// <summary>
    /// A set of terms to use for replacement commands
    /// </summary>
    public class ReplacementSet
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        /// <remarks>Database-generated</remarks>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Command to bind to
        /// </summary>
        [Required]
        public string Command { get; set; }

        /// <summary>
        /// Associated mappings
        /// </summary>
        public virtual ICollection<ReplacementMapping> Mappings { get; set; }
    }
}
