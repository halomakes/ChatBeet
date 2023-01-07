using ChatBeet.Utilities;
using Genbox.WolframAlpha;
using GravyBot.Commands;
using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Irc
{
    public class WolframCommandProcessor : CommandProcessor
    {
        private readonly WolframAlphaClient client;

        public WolframCommandProcessor(WolframAlphaClient client)
        {
            this.client = client;
        }

        [Command("wolfram {query}", Description = "Look something up on Wolfram Alpha")]
        [Command("ask {query}", Description = "Look something up on Wolfram Alpha")]
        public async IAsyncEnumerable<IClientMessage> Search([Required] string query)
        {
            var resultTask = client.ShortAnswerAsync(query);

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3));
            await Task.WhenAny(resultTask, timeoutTask);

            if (resultTask.IsCompleted)
            {
                var result = resultTask.Result;
                yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {result}");
            }
            else
            {
                // pinging page is taking too long, go ahead and give url then follow up with metadata later
                yield return new NoticeMessage(IncomingMessage.From, $"Still working on it, this is taking longer than usual...");
                var result = await resultTask;
                yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {result}");
            }
        }
    }
}
