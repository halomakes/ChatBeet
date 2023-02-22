using System.Collections.Generic;
using System.Threading.Tasks;
using BooruSharp.Search.Post;
using ChatBeet.Commands.Autocomplete;
using ChatBeet.Data;
using ChatBeet.Notifications;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class BooruCommandModule : ApplicationCommandModule
{
    private readonly BooruService _booru;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _users;

    public BooruCommandModule(BooruService booru, IMediator mediator, IUsersRepository users)
    {
        _booru = booru;
        _mediator = mediator;
        _users = users;
    }

    [SlashCommand("booru", "Get a random image from gelbooru matching tags")]
    private async Task GetPost(InteractionContext ctx, [Option("tags", "List of tags (space-separated)"), Autocomplete(typeof(BooruTagAutocompleteProvider))] string tags, [Option("rating", "Change this if you're horny")] Rating rating = Rating.General)
    {
        var (text, embed) = await GetResponseContent(tags, rating, (await _users.GetUserAsync(ctx.User)).Id);
        var response = new DiscordInteractionResponseBuilder()
            .WithContent(text);
        if (embed is not null)
            response = response.AddEmbed(embed);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
        var message = await ctx.GetOriginalResponseAsync();
        await _mediator.Publish(new BonkableMessageNotification(message));
    }

    public async Task<(string Content, DiscordEmbed? Embed)> GetResponseContent(string tags, Rating rating, Guid userId)
    {
        var tagList = tags.ToLower().Split(' ');
        if (tagList.Any())
        {
            var result = await _booru.GetRandomPostAsync(rating, userId, tagList);

            if (result is { } media)
            {
                await _booru.RecordTags(userId, tagList);

                if (media != default && media is { Rating: < Rating.Questionable })
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        ImageUrl = media.ImageUrl.ToString(),
                        Url = media.PageUrl.ToString()
                    };
                    return (@$"{media.Rating} - {FormatTags(media.Tags, tagList)}
{Formatter.EmbedlessUrl(media.PageUrl)}", embed);
                }
                else
                {
                    return (@$"{media.Rating} - {FormatTags(media.Tags, tagList)}
{Formatter.Spoiler(Formatter.EmbedlessUrl(media.PageUrl))}", null);
                }
            }
            else
            {
                return ($"Sorry, couldn't find anything for {string.Join(", ", tagList)}, ya perv. See available tags here: https://gelbooru.com/index.php?page=tags&s=list", null);
            }
        }
        else
        {
            return ($"Please specify tag/tags. See available list here: https://gelbooru.com/index.php?page=tags&s=list", null);
        }
    }

    private string FormatTags(IEnumerable<string> tags, IEnumerable<string> queriedTags) => string.Join(", ", tags
        .Select(t => queriedTags.Contains(t) ? Formatter.Bold(Formatter.Sanitize(t)) : Formatter.Sanitize(t)));

    [SlashCommand("astolfo", "Fill the void in your soul with an Astolfo picture")]
    public Task GetAstolfo(InteractionContext ctx) => GetPost(ctx, "astolfo_(fate)");
}