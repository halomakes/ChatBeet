using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Queuing
{
    public class MessageQueueService : IMessageQueueService
    {
        private readonly QueueConfigurationAccessor configurationAccessor;
        private const int MAX_HISTORY = 300;

        public MessageQueueService(QueueConfigurationAccessor configurationAccessor)
        {
            this.configurationAccessor = configurationAccessor;
        }

        private List<IQueuedMessageSource> queuedMessages = new List<IQueuedMessageSource>();
        private List<IQueuedMessageSource> messageHistory = new List<IQueuedMessageSource>();

        public event EventHandler MessageAdded;

        public List<IQueuedMessageSource> ViewAll() => queuedMessages;
        public List<IQueuedMessageSource> GetHistory() => messageHistory;

        public List<IQueuedMessageSource> PopAll()
        {
            var messages = queuedMessages;
            queuedMessages = new List<IQueuedMessageSource>();
            return messages;
        }

        public void Push(IQueuedMessageSource message)
        {
            queuedMessages.Add(message);
            messageHistory.Add(message);
            TrimHistory();
            OnMessageAdded(EventArgs.Empty);
        }

        private void OnMessageAdded(EventArgs e)
        {
            MessageAdded?.Invoke(this, e);
        }

        private void TrimHistory()
        {
            var overflow = MAX_HISTORY - messageHistory.Count;
            if (overflow > 0)
                messageHistory = messageHistory.Skip(overflow).Take(MAX_HISTORY).ToList();
        }
    }
}
