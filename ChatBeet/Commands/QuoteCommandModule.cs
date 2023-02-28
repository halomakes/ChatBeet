using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public partial class QuoteCommandModule : ApplicationCommandModule
{
    private readonly IQuoteRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly IUsersRepository _users;

    public QuoteCommandModule(IQuoteRepository repository, IConfiguration configuration, IUsersRepository users)
    {
        _repository = repository;
        _configuration = configuration;
        _users = users;
    }

    [GeneratedRegex(@"^[\w\d]+(?:-[\w\d]+)*$")]
    private partial Regex SlugRgx();

    [SlashCommand("quote", "Store recent messages as a quote")]
    public async Task StoreQuote(InteractionContext ctx,
        [Option("id", "ID to give this quote - letters, numbers, and hyphens only"), MaximumLength(200)]
        string slug,
        [Option("scrollback", "Number of messages to include in the quote"), Maximum(99), Minimum(1)]
        long scrollback)
    {
        if (!SlugRgx().IsMatch(slug))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"ID must consist of only letters, numbers, and hyphens. It cannot start or end with a hyphen. ")
                .AsEphemeral());
            return;
        }

        var isUsed = await _repository.Quotes.AnyAsync(q => q.GuildId == ctx.Guild.Id && q.Slug == slug);
        if (isUsed)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Quote already exists with ID {Formatter.Bold(slug)}.")
                .AsEphemeral());
            return;
        }

        await ctx.DeferAsync();
        var messages = await ctx.Channel.GetMessagesAsync((int)scrollback + 1);
        var user = await _users.GetUserAsync(ctx.User);
        var messageUsers = new List<User>();
        foreach (var author in messages.Select(m => m.Author).DistinctBy(a => a.Id))
            messageUsers.Add(await _users.GetUserAsync(author));
        var response = await ctx.GetOriginalResponseAsync();
        var quote = new Quote
        {
            Slug = slug,
            GuildId = ctx.Guild.Id,
            ChannelName = ctx.Channel.Name,
            SavedById = user.Id,
            CreatedAt = DateTime.UtcNow,
            Messages = messages
                .OrderBy(m => m.Timestamp)
                .Where(m => m.Id != response.Id)
                .Select(m => new QuoteMessage
                {
                    Author = messageUsers.First(u => u.Discord!.Id == m.Author.Id),
                    Embeds = m.Embeds.Count,
                    Attachments = m.Attachments.Count,
                    Content = m.Content,
                    CreatedAt = m.Timestamp.DateTime
                }).ToList()
        };
        _repository.Quotes.Add(quote);
        await _repository.SaveChangesAsync();

        var webUrl = _configuration.GetValue<string>("CanonicalUrl");
        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
            .WithContent($"Quote created at {webUrl}/quotes/{slug}?guild={ctx.Guild.Id}"));
    }
}