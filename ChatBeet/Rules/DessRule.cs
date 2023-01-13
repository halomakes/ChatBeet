using ChatBeet.Commands.Discord;
using ChatBeet.Commands.Irc;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public partial class DessRule : CommandAliasRule<BooruCommandProcessor>, IAsyncMessageRule<MessageCreateEventArgs>
    {
        private readonly BooruService _booru;

        [GeneratedRegex(@"^!dess$", RegexOptions.IgnoreCase)]
        private static partial Regex rgx();

        public DessRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider, BooruService booru) : base(options, serviceProvider)
        {
            Pattern = rgx();
            _booru = booru;
        }

        protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match match, BooruCommandProcessor commandProcessor)
        {
            yield return await commandProcessor.GetRandomSafePost("akatsuki_kirika");
        }

        public async IAsyncEnumerable<IClientMessage> RespondAsync(MessageCreateEventArgs incomingMessage)
        {
            var commandModule = new BooruCommandModule(_booru);
            var (text, embed) = await commandModule.GetResponseContent("akatsuki_kirika", true, incomingMessage.Author.DiscriminatedUsername());
            var builder = new DiscordMessageBuilder()
                .WithContent(text);
            if (embed is not null)
                builder = builder.WithEmbed(embed);
            await incomingMessage.Message.RespondAsync(builder);
            yield break;
        }

        public bool Matches(MessageCreateEventArgs incomingMessage) => rgx().IsMatch(incomingMessage.Message.Content);
    }
}
