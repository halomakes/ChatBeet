using ChatBeet.Queuing.Rules;

namespace ChatBeet.Queuing
{
    public class OutputMessage
    {
        public string Content { get; set; }
        public OutputType OutputType { get; set; }
        public string Channel { get; set; }
    }
}
