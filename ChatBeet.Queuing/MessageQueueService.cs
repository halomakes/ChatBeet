using System;
using System.Collections.Generic;

namespace ChatBeet.Queuing
{
    public class MessageQueueService : IMessageQueueService
    {
        private QueueConfigurationAccessor configurationAccessor;

        public MessageQueueService(QueueConfigurationAccessor configurationAccessor)
        {
            this.configurationAccessor = configurationAccessor;
        }

        private List<IQueuedMessageSource> queuedMessages = new List<IQueuedMessageSource>();

        public event EventHandler MessageAdded;

        public List<IQueuedMessageSource> ViewAll() => queuedMessages;

        public List<IQueuedMessageSource> PopAll()
        {
            var messages = queuedMessages;
            queuedMessages = new List<IQueuedMessageSource>();
            return messages;
        }

        public void Push(IQueuedMessageSource message)
        {
            queuedMessages.Add(message);
            OnMessageAdded(EventArgs.Empty);
        }

        private void OnMessageAdded(EventArgs e)
        {
            MessageAdded?.Invoke(this, e);
        }
    }
}
