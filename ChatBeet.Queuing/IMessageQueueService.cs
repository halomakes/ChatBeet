using System;
using System.Collections.Generic;

namespace ChatBeet.Queuing
{
    public interface IMessageQueueService
    {
        List<OutputMessage> PopAll();
        void Push(IQueuedMessageSource message);
        IEnumerable<OutputMessage> ViewAll();
        event EventHandler MessageAdded;
        IEnumerable<IQueuedMessageSource> GetHistory();
        IEnumerable<OutputMessage> GetOutputHistory();
        void PushRaw(OutputMessage message);
        void Remove(OutputMessage message);
    }
}