using System;
using System.Collections.Generic;

namespace ChatBeet.Queuing
{
    public interface IMessageQueueService
    {
        List<OutputMessage> PopAll();
        void Push(IQueuedMessageSource message);
        List<OutputMessage> ViewAll();
        event EventHandler MessageAdded;
        List<IQueuedMessageSource> GetHistory();
        List<OutputMessage> GetOutputHistory();
    }
}