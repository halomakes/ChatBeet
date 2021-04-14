using ChatBeet.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Data.Entities
{
    public class FixedTimeRange
    {
        [Key, Required, RegularExpression(@"^[\w\d]+(?:-[\w\d]+)?$", ErrorMessage = "Key must contain only letters, digits, and hyphens and cannot begin or end with a hyphen."), MaxLength(30)]
        public string Key { get; set; }

        [Required, RegularExpression(@"\{percentage\}", ErrorMessage = "Template must contain a {percentage} token."), MaxLength(200)]
        public string Template { get; set; } = "This custom range is {percentage} complete.";

        [MaxLength(200)]
        public string BeforeRangeMessage { get; set; }

        [MaxLength(200)]
        public string AfterRangeMessage { get; set; }

        [Required, Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required, Display(Name = "End Date"), DateGreaterThan(nameof(StartDate))]
        public DateTime EndDate { get; set; }
    }
}
