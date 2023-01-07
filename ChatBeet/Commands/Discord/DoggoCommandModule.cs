using DogApi;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class DoggoCommandModule : ApplicationCommandModule
{
    private readonly DogApiClient client;

    public DoggoCommandModule(DogApiClient client)
    {
        this.client = client;
    }

    [SlashCommand("doggo", "Get a random dog picture. 🐕")]
    public async Task GetRandomDoggo(InteractionContext ctx)
    {
        try
        {
            var image = (await client.SearchImagesAsync(breedsOnly: true, limit: 1)).FirstOrDefault();
            if (image != default)
            {
                string textContent;
                var breed = image.Breeds.FirstOrDefault();
                textContent = GetBreedInfo(breed);

                var embed = new DiscordEmbedBuilder
                {
                    ImageUrl = image.Url.ToString()
                };
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"{image.Url} {GetBreedInfo(image.Breeds?.First())}")
                    .AddEmbed(embed));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"Sorry, couldn't find any doggos right now.  Maybe they're all taking a nap?"));
            }
        }
        catch (Exception)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Sorry, couldn't find any doggos right now.  Maybe they're all taking a nap?"));
            throw;
        }
    }

    private static string GetBreedInfo(Breed breed)
    {
        return string.Join(string.Empty, GetSegments());

        IEnumerable<string> GetSegments()
        {
            yield return Formatter.Bold(breed.Name);
            if (!string.IsNullOrWhiteSpace(breed.BredFor))
                yield return $" — bred for {breed.BredFor}";
            if (!string.IsNullOrWhiteSpace(breed.Temperament))
                yield return $" | {breed.Temperament}";
        }
    }
}