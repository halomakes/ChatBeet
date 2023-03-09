using System.Threading.Tasks;
using ChatBeet.Commands.Autocomplete;
using ChatBeet.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Commands;

[SlashCommandGroup("tag", "Commands related to tagging users with info")]
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class UserTagsCommandModule : ApplicationCommandModule
{
    private readonly IUsersRepository _userRepo;

    public UserTagsCommandModule(IUsersRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [SlashCommand("set", "Set a tag on a user")]
    public async Task SetTag(
        InteractionContext ctx,
        [Option("user", "User to set tag on")] DiscordUser user,
        [Option("tag", "Tag to set"), MaximumLength(50), Autocomplete(typeof(MetadataAutocompleteProvider))]
        string tag,
        [Option("value", "Value to assign"), MaximumLength(100)]
        string value
    )
    {
        var dbUser = await _userRepo.GetUserAsync(user);
        var invokingUser = await _userRepo.GetUserAsync(ctx.User);
        var existingTag = await _userRepo.Metadata
            .Where(m => m.GuildId == ctx.Guild.Id)
            .Where(m => m.UserId == dbUser.Id)
            .FirstOrDefaultAsync(m => m.Key.ToLower() == tag.ToLower());
        if (existingTag is null)
        {
            _userRepo.Metadata.Add(new()
            {
                GuildId = ctx.Guild.Id,
                Key = tag.ToLower(),
                UserId = dbUser.Id,
                Value = value,
                AuthorId = invokingUser.Id
            });
        }
        else
        {
            existingTag.Value = value;
            existingTag.AuthorId = invokingUser.Id;
        }

        await _userRepo.SaveChangesAsync();
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"Set {tag} on {Formatter.Mention(user)} to {value}."));
    }
    
    [SlashCommand("get", "Set a tag on a user")]
    public async Task GetTag(
        InteractionContext ctx,
        [Option("user", "User to set tag on")] DiscordUser user,
        [Option("tag", "Tag to set"), MaximumLength(50), Autocomplete(typeof(MetadataAutocompleteProvider))]
        string tag
    )
    {
        var existingTag = await _userRepo.Metadata
            .Where(m => m.GuildId == ctx.Guild.Id)
            .Where(m => m.User!.Discord!.Id == user.Id)
            .FirstOrDefaultAsync(m => m.Key.ToLower() == tag.ToLower());
        if (existingTag is null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)} has no value set for {Formatter.Bold(tag)}."));
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Bold(tag)} for {Formatter.Mention(user)} is set to {existingTag.Value}."));
        }
    }
}