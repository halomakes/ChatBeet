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

        private readonly List<OutputMessage> queuedMessages = new List<OutputMessage>();
        private List<IQueuedMessageSource> messageHistory = new List<IQueuedMessageSource>();
        private List<OutputMessage> outputHistory = new List<OutputMessage>();

        public event EventHandler MessageAdded;

        public List<OutputMessage> ViewAll() => queuedMessages;
        public List<IQueuedMessageSource> GetHistory() => messageHistory;
        public List<OutputMessage> GetOutputHistory() => outputHistory;

        private void ApplyRules(IQueuedMessageSource message)
        {
            foreach (var rule in configurationAccessor.GetRules())
            {
                try
                {
                    if (rule.Condition.Matches(message))
                    {
                        AddOutput(new OutputMessage
                        {
                            Channel = rule.TargetChannel,
                            Content = rule.Output.GetOutput(message),
                            OutputType = rule.Type
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to apply rule: ", e);
                }
            }
        }

        public List<OutputMessage> PopAll()
        {
            var messages = queuedMessages;
            queuedMessages.Clear();
            return messages;
        }

        private void AddOutput(OutputMessage message)
        {
            queuedMessages.Add(message);
            outputHistory.Add(message);
            TrimOutputHistory();
            OnMessageAdded(EventArgs.Empty);
        }

        public void Push(IQueuedMessageSource message)
        {
            messageHistory.Add(message);
            TrimHistory();
            ApplyRules(message);
        }

        private void OnMessageAdded(EventArgs e)
        {
            MessageAdded?.Invoke(this, e);
        }

        private void TrimHistory()
        {
            var overflow = messageHistory.Count - MAX_HISTORY;
            if (overflow > 0)
                messageHistory = messageHistory.Skip(overflow).Take(MAX_HISTORY).ToList();
        }

        private void TrimOutputHistory()
        {
            var overflow = outputHistory.Count - MAX_HISTORY;
            if (overflow > 0)
                outputHistory = outputHistory.Skip(overflow).Take(MAX_HISTORY).ToList();
        }
    }
}
