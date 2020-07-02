using System.Collections.Generic;

namespace NetIRC.Messages
{
    public interface IClientMessage
    {
        IEnumerable<string> Tokens { get; }
    }
}
