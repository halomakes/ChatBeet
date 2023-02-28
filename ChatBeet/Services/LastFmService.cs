using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class LastFmService
{
    private readonly LastfmClient _client;

    public LastFmService(LastfmClient client)
    {
        _client = client;
    }

    public async Task<LastTrack?> GetTrackInfo(string track, string artist)
    {
        var response = await _client.Track.GetInfoAsync(track, artist);
        return response?.Content;
    }

    public async Task<LastArtist?> GetArtistInfo(string artist)
    {
        var response = await _client.Artist.GetInfoAsync(artist);
        return response?.Content;
    }
}