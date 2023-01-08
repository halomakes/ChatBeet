using ChatBeet.Commands.Discord.Autocomplete;
using ChatBeet.Services;
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

    [SlashCommand("booru", "Get a random image from gelbooru matching tags (safe only).")]
    private async Task GetPost(InteractionContext ctx, [Option("tags", "List of tags (space-separated)"), Autocomplete(typeof(BooruTagAutocompleteProvider))] string tags, [Option("safe-only", "Turn this off if you're horny")] bool safeOnly = true)
    {
        var tagList = tags.ToLower().Split(' ');
        if (tagList.Any())
        {
            var result = await booru.GetRandomPostAsync(safeOnly, ctx.User.Username, tagList);

            if (result is BooruService.MediaSearchResult media)
            {
                await booru.RecordTags(ctx.User.Username, tagList);

                if (media != default && media is { Rating: < BooruSharp.Search.Post.Rating.Explicit })
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        ImageUrl = media.ImageUrl.ToString(),
                        Url = media.PageUrl.ToString()
                    };
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(@$"{media.Rating} - {FormatTags(media.Tags, tagList)}
{Formatter.EmbedlessUrl(media.PageUrl)}")
                    .AddEmbed(embed));
                }
                else
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(@$"{media.Rating} - {FormatTags(media.Tags, tagList)}
{Formatter.Spoiler(Formatter.EmbedlessUrl(media.PageUrl))}"));
                }
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"Sorry, couldn't find anything for {string.Join(", ", tagList)}, ya perv. See available tags here: https://gelbooru.com/index.php?page=tags&s=list")
               );
            }
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Please specify tag/tags. See available list here: https://gelbooru.com/index.php?page=tags&s=list")
            );
        }
    }

    private string FormatTags(IEnumerable<string> tags, IEnumerable<string> queriedTags) => string.Join(", ", tags
        .Select(t => queriedTags.Contains(t) ? Formatter.Bold(Formatter.Sanitize(t)) : Formatter.Sanitize(t)));

    [SlashCommand("astolfo", "Fill the void in your soul with an Astolfo picture.")]
    public Task GetAstolfo(InteractionContext ctx) => GetPost(ctx, "astolfo_(fate)", true);
}