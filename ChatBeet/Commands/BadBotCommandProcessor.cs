using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using System;
using System.Collections.Generic;

namespace ChatBeet.Commands
{
    public class BadBotCommandProcessor : CommandProcessor
    {
        private static DateTime? lastReactionTime = null;
        private static readonly TimeSpan debounce = TimeSpan.FromSeconds(20);

        [Command("bad bot", Description = "Hurt ChatBeet's feelings.")]
        [Command("shit bot", Description = "Really hurt ChatBeet's feelings.")]
        public IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            if (!lastReactionTime.HasValue || (DateTime.Now - lastReactionTime.Value) > debounce)
            {
                yield return new PrivateMessage(incomingMessage.GetResponseTarget(), "*sad bot noises*");
            }
            lastReactionTime = DateTime.Now;
        }
    }
}
