using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Rules
{
    public class EmojifyRule : NickLookupRule
    {
        private static Dictionary<string, string> mappings = new Dictionary<string, string>
        {
            {"b", "🅱" },
            {"cat", "🐱" },
            {"eggplant","🍆" },
            {"butt","🍑" },
            {"fire","🔥" },
            {"die","💀" },
            {"in","➡" },
            {"netflix","🎥🍿" },
            {"chill","❄" },
            {"dick","🍆"},
            {"oral","😮" },
            {"poop","💩" },
            {"ready", "🚀" },
            {"clap", "👏" },
            {"lol","😂" },
            {"k", "👌" },
            {"like","👍" },
            {"yes","👍" },
            {"wat","❓" },
            {"?!","⁉" },
            {"!?","⁉" },
            {"?","❓" },
            {"<3", "♥" },
            {"carrot","🥕" },
            {"say", "🗨" },
            {"a", "🅰" },
            {"o","🅾"},
            {"stop","🛑"},
            {"hello","👋"},
            {"hi", "🙋‍♂️"},
            {"boy", "🚹"},
            {"man", "🚹"},
            {"girl", "🚺"},
            {"woman", "🚺"},
            {"egg","🥚"},
            {"think","🧠"},
            {"people","🤼"},
            {"right","👉"},
            {"left","👈"},
            {"up","👆"},
            {"down","👇"}
        };

        public EmojifyRule(MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options) : base(messageQueueService, options)
        {
            CommandName = "emojify";
        }

        protected override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var content = lookupMessage.Message;
            foreach (var pair in mappings.OrderByDescending(m => m.Key.Length))
                content = content.Replace(pair.Key, pair.Value, true, DtellaRuleConfiguration.Culture);

            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {content}");
        }
    }
}