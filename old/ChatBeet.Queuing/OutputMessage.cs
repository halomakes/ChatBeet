using ChatBeet.Queuing.Rules;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Queuing
{
    public class OutputMessage
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public OutputType OutputType { get; set; }
        [Required]
        public string Target { get; set; }
    }
}
