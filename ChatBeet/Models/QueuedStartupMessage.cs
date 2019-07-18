using ChatBeet.Queuing;
using System;
using System.Runtime.InteropServices;

namespace ChatBeet.Models
{
    public class QueuedStartupMessage : IQueuedMessageSource
    {
        public QueuedStartupMessage()
        {
            Target = "System";
            Source = "startup";
            Title = "ChatBeet started up";
            Body = $"ChatBeet is running on {Environment.MachineName} {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture} using {RuntimeInformation.FrameworkDescription}";
            TimeGenerated = DateTime.UtcNow;
        }

        public string Target { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime TimeGenerated { get; set; }
    }
}
