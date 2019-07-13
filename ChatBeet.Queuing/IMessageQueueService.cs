using System;
using System.Collections.Generic;

namespace ChatBeet.Queuing
{
    public interface IMessageQueueService
    {
        List<IQueuedMessageSource> PopAll();
        void Push(IQueuedMessageSource message);
        List<IQueuedMessageSource> ViewAll();
        event EventHandler MessageAdded;
    }
}