using ChatBeet.Utilities;
using DogApi;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class DoggoCommandProcessor : CommandProcessor
    {
        private readonly DogApiClient client;

        public DoggoCommandProcessor(DogApiClient client)
        {
            this.client = client;
        }

        [Command("dog", Description = "Get a random dog picture. 🐕")]
        [Command("doggo", Description = "Get a random dog picture. 🐕")]
        [RateLimit(15, TimeUnit.Second)]
        public async Task<IClientMessage> GetRandomDoggo()
        {
            try
            {
                var image = (await client.SearchImagesAsync(breedsOnly: true, limit: 1)).FirstOrDefault();
                if (image != default)
                {
                    if (image.Breeds.Count() == 1)
                    {
                        var breed = image.Breeds.FirstOrDefault();
                        return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{image.Url} {GetBreedInfo(breed)}");
                    }
                    else
                    {
                        var breedsString = string.Join(", ", image.Breeds.Select(b => $"{IrcValues.BOLD}{b.Name}{IrcValues.RESET}"));
                        return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{image.Url} {breedsString}");
                    }
                }
                return new NoticeMessage(IncomingMessage.From, "Sorry, couldn't find any doggos right now.  Maybe they're all taking a nap?");
            }
            catch (Exception e)
            {
                return new NoticeMessage(IncomingMessage.From, "Sorry, couldn't find any doggos right now.  Maybe they're all taking a nap?");
                throw;
            }
        }

        private string GetBreedInfo(Breed breed)
        {
            return string.Join(string.Empty, GetSegments());

            IEnumerable<string> GetSegments()
            {
                yield return $"{IrcValues.BOLD}{breed.Name}{IrcValues.RESET}";
                if (!string.IsNullOrWhiteSpace(breed.BredFor))
                    yield return $" — bred for {breed.BredFor}";
                if (!string.IsNullOrWhiteSpace(breed.Temperament))
                    yield return $" | {breed.Temperament}";
            }
        }
    }
}
