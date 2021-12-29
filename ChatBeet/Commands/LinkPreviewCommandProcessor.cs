using ChatBeet.Attributes;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class LinkPreviewCommandProcessor : CommandProcessor
    {
        private readonly MessageQueueService messageQueue;
        private readonly LinkPreviewService previewService;

        public LinkPreviewCommandProcessor(MessageQueueService messageQueue, LinkPreviewService previewService)
        {
            this.messageQueue = messageQueue;
            this.previewService = previewService;
        }

        [Command("preview {uri}", Description = "Parse a link preview from a URI."), ChannelOnly]
        public async Task<IClientMessage> PreviewLink([Uri] string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                var rgx = new Regex(RegexUtils.Uri, RegexOptions.IgnoreCase);
                var lookupMessage = messageQueue.GetLatestMessage(IncomingMessage.To, rgx);
                if (lookupMessage == default)
                    return new NoticeMessage(IncomingMessage.From, $"Couldn't find a URI to preview.");
                var match = rgx.Match(lookupMessage.Message);
                uri = match.Value;
            }

            if (Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri) || Uri.TryCreate($"https://{uri}", UriKind.Absolute, out parsedUri))
            {
                try
                {
                    var meta = await previewService.GetDocumentAsync(parsedUri);
                    if (meta != default)
                        return new PrivateMessage(IncomingMessage.GetResponseTarget(), meta.ToIrcSummary(maxDescriptionLength: 400));
                    return new NoticeMessage(IncomingMessage.From, $"Sorry, I couldn't parse details out of that page.");
                }
                catch (Exception e)
                {
                    messageQueue.Push(e);
                    return new NoticeMessage(IncomingMessage.From, $"Couldn't fetch {IrcValues.ITALIC}{parsedUri}{IrcValues.RESET}.  Make sure it's absolute and publicly accessible.");
                }
            }

            return new NoticeMessage(IncomingMessage.From, $"Couldn't parse that URI.  Make sure it's absolute and publicly accessible.");
        }
    }
}
