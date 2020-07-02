using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System.Threading.Tasks;

namespace DtellaRules.Services
{
    public class LastFmService
    {
        private readonly LastfmClient client;

        public LastFmService(LastfmClient client)
        {
            this.client = client;
        }

        public async Task<LastTrack> GetTrackInfo(string track, string artist)
        {
            var response = await client.Track.GetInfoAsync(track, artist);
            return response?.Content;
        }

        public async Task<LastArtist> GetArtistInfo(string artist)
        {
            var response = await client.Artist.GetInfoAsync(artist);
            return response?.Content;
        }
    }
}
