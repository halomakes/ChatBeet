using ChatBeet.Data;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class MemoryCellCommandProcessor : CommandProcessor
    {
        private readonly MemoryCellContext dbContext;

        public MemoryCellCommandProcessor(MemoryCellContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [Command("whodef {key}")]
        public async Task<IClientMessage> GetAuthor(string key)
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

        [Command("recall {key}")]
        public async Task<IClientMessage> GetCell(string key)
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

        private IClientMessage NotFound(string key) => new PrivateMessage(
            IncomingMessage.GetResponseTarget(),
            $"I don't have anything for {IrcValues.BOLD}{key}{IrcValues.RESET}."
        );
    }
}
