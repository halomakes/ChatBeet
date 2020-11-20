using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class MemoryCellCommandProcessor : CommandProcessor
    {
        private readonly MemoryCellContext dbContext;
        private readonly MessageQueueService queue;

        public MemoryCellCommandProcessor(MemoryCellContext dbContext, MessageQueueService queue)
        {
            this.dbContext = dbContext;
            this.queue = queue;
        }

        [Command("whodef {key}", Description = "Check who set a peasant definition.")]
        public async Task<IClientMessage> GetAuthor([Required] string key)
        {
            var cell = await dbContext.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());

            if (cell != null)
                return new PrivateMessage(
                    IncomingMessage.GetResponseTarget(),
                    $"{IrcValues.BOLD}{cell.Key}{IrcValues.RESET} was set by {IrcValues.BOLD}{cell.Author}{IrcValues.RESET}"
                );
            else
                return NotFound(key);
        }

        [Command("recall {key}", Description = "Get the value of a peasant definition.")]
        public async Task<IClientMessage> GetCell([Required] string key)
        {
            var cell = await dbContext.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());

            if (cell != null)
                return new PrivateMessage(
                    IncomingMessage.GetResponseTarget(),
                    $"{IrcValues.BOLD}{cell.Key}{IrcValues.RESET}: {cell.Value}"
                );
            else
                return NotFound(key);
        }

        [Command("remember {key}={value}", Description = "Create or replace a peasant definition.")]
        public async IAsyncEnumerable<IClientMessage> SetCell([Required] string key, [Required] string value)
        {
            // want to put as many equals on the right side as we can
            var normalized = ParameterHelper.ForceCharacterOnRight((key, value), '=');
            key = normalized.Item1?.Trim();
            value = normalized.Item2?.Trim();

            if (string.IsNullOrEmpty(key))
            {
                yield return new PrivateMessage(
                        IncomingMessage.GetResponseTarget(),
                        $"{IncomingMessage.From}: provide a name to define."
                    );
            }
            else if (string.IsNullOrEmpty(value))
            {
                yield return new PrivateMessage(
                        IncomingMessage.GetResponseTarget(),
                        $"{IncomingMessage.From}: provide a value to set for {IrcValues.BOLD}{key}{IrcValues.RESET}."
                    );
            }
            else
            {
                var existingCell = await dbContext.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());
                if (existingCell != null)
                {
                    dbContext.MemoryCells.Remove(existingCell);
                    await dbContext.SaveChangesAsync();
                }

                dbContext.MemoryCells.Add(new MemoryCell
                {
                    Author = IncomingMessage.From,
                    Key = key,
                    Value = value
                });
                await dbContext.SaveChangesAsync();

                yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Got it! 👍");

                if (existingCell != null)
                {
                    yield return new PrivateMessage(
                        IncomingMessage.GetResponseTarget(),
                        $"Previous value was {IrcValues.BOLD}{existingCell.Value}{IrcValues.RESET}, set by {existingCell.Author}."
                    );
                }

                if (!IncomingMessage.IsChannelMessage)
                {
                    queue.Push(new DefinitionChange
                    {
                        Key = key,
                        NewNick = IncomingMessage.From,
                        NewValue = value,
                        OldNick = existingCell?.Author,
                        OldValue = existingCell?.Value
                    });
                }
            }
        }

        private IClientMessage NotFound(string key) => new PrivateMessage(
            IncomingMessage.GetResponseTarget(),
            $"I don't have anything for {IrcValues.BOLD}{key}{IrcValues.RESET}."
        );
    }
}
