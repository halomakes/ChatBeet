using System;

namespace ChatBeet.Queuing
{
    public interface IQueuedMessageSource
    {
        string Target { get; set; }
        string Source { get; set; }
        string Title { get; set; }
        string Body { get; set; }
        DateTime TimeGenerated { get; set; }
    }
}
