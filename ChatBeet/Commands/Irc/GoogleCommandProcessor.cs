using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Irc;

public class GoogleCommandProcessor : CommandProcessor
{
    private readonly LinkPreviewService previewSerivce;
    private readonly GoogleSearchService searchService;

    public GoogleCommandProcessor(GoogleSearchService searchService, LinkPreviewService previewSerivce)
    {
        this.searchService = searchService;
        this.previewSerivce = previewSerivce;
    }

    [Command("google {query}", Description = "Look something up on Google using I'm Feeling Lucky")]
    [Command("feelinglucky {query}", Description = "Look something up on Google using I'm Feeling Lucky")]
    public async IAsyncEnumerable<IClientMessage> Search([Required] string query)
    {
        var resultLink = await searchService.GetFeelingLuckyResultAsync(query);
        if (!resultLink.Host.Contains("google.com", StringComparison.OrdinalIgnoreCase))
        {
            // not a google link, try to generate preview
            var metaTask = previewSerivce.GetDocumentAsync(resultLink);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));
            await Task.WhenAny(metaTask, timeoutTask);

            if (metaTask.IsCompleted)
            {
                var meta = metaTask.Result;
                yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {resultLink} {meta.ToIrcSummary(maxDescriptionLength: 200)}");
            }
            else
            {
                // pinging page is taking too long, go ahead and give url then follow up with metadata later
                yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {resultLink}");
                var meta = await metaTask;
                yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), meta.ToIrcSummary(maxDescriptionLength: 400));
            }
        }
        else
        {
            yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {resultLink}");
        }
    }
}