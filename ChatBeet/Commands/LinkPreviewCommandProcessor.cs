using ChatBeet.Attributes;
using ChatBeet.Converters;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System;
using System.ComponentModel;
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
        public async Task<IClientMessage> PreviewLink([Uri, TypeConverter(typeof(UrlTypeConverter))] Uri uri)
        {
            if (uri is null)
            {
                var rgx = new Regex(RegexUtils.Uri, RegexOptions.IgnoreCase);
                var lookupMessage = messageQueue.GetLatestMessage(IncomingMessage.To, rgx);
                if (lookupMessage == default)
                    return new NoticeMessage(IncomingMessage.From, $"Couldn't find a URI to preview.");
                var match = rgx.Match(lookupMessage.Message);
                if (Uri.TryCreate(match.Value, UriKind.Absolute, out var historic))
                    uri = historic;
            }

            if (uri is not null)
            {
                try
                {
                    var meta = await previewService.GetDocumentAsync(uri);
                    if (meta != default)
                    {
                        var summary = meta.ToIrcSummary(maxDescriptionLength: 400);
                        if (!string.IsNullOrEmpty(summary))
                            return new PrivateMessage(IncomingMessage.GetResponseTarget(), summary);
                    }
                    return new NoticeMessage(IncomingMessage.From, $"Sorry, I couldn't parse details from that page.");
                }
                catch (Exception e)
                {
                    messageQueue.Push(e);
                    return new NoticeMessage(IncomingMessage.From, $"Couldn't fetch {IrcValues.ITALIC}{uri}{IrcValues.RESET}.  Make sure it's absolute and publicly accessible.");
                }
            }

            return new NoticeMessage(IncomingMessage.From, $"Couldn't parse that URI.  Make sure it's absolute and publicly accessible.");
        }
    }
}
