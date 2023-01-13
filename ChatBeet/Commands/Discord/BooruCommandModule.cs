using ChatBeet.Commands.Discord.Autocomplete;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class BooruCommandModule : ApplicationCommandModule
{
    private readonly BooruService booru;

    public BooruCommandModule(BooruService booru)
    {
        this.booru = booru;
    }

    [SlashCommand("booru", "Get a random image from gelbooru matching tags")]
    private async Task GetPost(InteractionContext ctx, [Option("tags", "List of tags (space-separated)"), Autocomplete(typeof(BooruTagAutocompleteProvider))] string tags, [Option("safe-only", "Turn this off if you're horny")] bool safeOnly = true)
    {
        var (text, embed) = await GetResponseContent(tags, safeOnly, ctx.User.DiscriminatedUsername());
        var response = new DiscordInteractionResponseBuilder()
                    .WithContent(text);
        if (embed is not null)
            response = response.AddEmbed(embed);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
    }

    public async Task<(string Content, DiscordEmbed? Embed)> GetResponseContent(string tags, bool safeOnly, string username)
    {
        var tagList = tags.ToLower().Split(' ');
        if (tagList.Any())
        {
            var result = await booru.GetRandomPostAsync(safeOnly, username, tagList);

            if (result is BooruService.MediaSearchResult media)
            {
                if (username is not null)
                    await booru.RecordTags(username, tagList);

                if (media != default && media is { Rating: < BooruSharp.Search.Post.Rating.Explicit })
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
    public Task GetAstolfo(InteractionContext ctx) => GetPost(ctx, "astolfo_(fate)", true);
}