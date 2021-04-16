using ChatBeet.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Data.Entities
{
    public class FixedTimeRange
    {
        /// <summary>
        /// Key of time range
        /// </summary>
        /// <remarks>Can be alphanumeric with hyphens in the middle</remarks>
        [Key, Required, RegularExpression(@"^[\w\d]+(?:-[\w\d]+)?$", ErrorMessage = "Key must contain only letters, digits, and hyphens and cannot begin or end with a hyphen."), MaxLength(30)]
        public string Key { get; set; }

        /// <summary>
        /// Format template for responses
        /// </summary>
        /// <remarks>Any instances of {percentage} will be replaced</remarks>
        [Required, RegularExpression(@".*\{percentage\}.*", ErrorMessage = "Template must contain a {{percentage}} token."), MaxLength(200)]
        public string Template { get; set; } = "This custom range is {percentage} complete.";

        /// <summary>
        /// Message to display if period has not yet begun
        /// </summary>
        /// <remarks>Will show 0% instead if not specified</remarks>
        [MaxLength(200)]
        public string BeforeRangeMessage { get; set; }

        /// <summary>
        /// Message to display if period has already ended
        /// </summary>
        /// <remarks>Will show 100% instead if not specified</remarks>
        [MaxLength(200)]
        public string AfterRangeMessage { get; set; }

        /// <summary>
        /// Beginning of time range
        /// </summary>
        [Required, Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End of time range
        /// </summary>
        [Required, Display(Name = "End Date"), DateGreaterThan(nameof(StartDate))]
        public DateTime EndDate { get; set; }
    }
}
