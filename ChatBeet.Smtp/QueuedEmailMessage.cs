using ChatBeet.Queuing;
using MimeKit;
using System;
using System.Linq;

namespace ChatBeet.Smtp
{
    public class QueuedEmailMessage : IQueuedMessageSource
    {
        public string Target { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime TimeGenerated { get; set; }

        public static QueuedEmailMessage FromMimeMessage(MimeMessage msg) => new QueuedEmailMessage
        {
            Body = msg?.TextBody,
            Source = "email:" + (((MailboxAddress)msg.From?.FirstOrDefault())?.Address ?? string.Empty),
            Target = ((MailboxAddress)msg.To?.FirstOrDefault())?.Address,
            TimeGenerated = msg.Date.DateTime,
            Title = msg.Subject
        };
    }
}
