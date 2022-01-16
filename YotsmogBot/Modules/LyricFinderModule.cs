using System.Xml.Linq;
using AHpx.Extensions.JsonExtensions;
using AHpx.Extensions.StringExtensions;
using Flurl;
using Flurl.Http;
using Mirai.Net.Data.Commands;
using YotsmogBot.Data;

namespace YotsmogBot.Modules;

public class LyricFinderModule : IModule
{
    private readonly string? ApiKey = ConfigUtils.GetApiKeyAsync("musixmatch").GetAwaiter().GetResult();
    
    public async void Execute(MessageReceiverBase @base)
    {
        var receiver = @base.Concretize<GroupMessageReceiver>();

        try
        {
            if (receiver.MessageChain.ContainsCommand(new[] { "/lyric" }))
            {
                var entity = receiver.MessageChain.GetPlainMessage().ParseCommand<LyricInfo>();
                var lyric = await GetLyricAsync(entity.TrackName, entity.ArtistName);

                await receiver.SendMessageAsync(lyric);
            }
        }
        catch (Exception e)
        {
            await receiver.SendMessageAsync($"获取失败了! 因为: {e.Message}");   
            new Logger().Log(e);
        }
    }

    public bool? IsEnable { get; set; }

    [CommandEntity(Name = "lyric")]
    class LyricInfo
    {
        [CommandArgument(Name = "artist", IsRequired = true)]
        public string ArtistName { get; set; }

        [CommandArgument(Name = "track", IsRequired = true)]
        public string TrackName { get; set; }
    }
    
    #region Music services

    private async Task<ArtistInfo> GetArtistInfoAsync(string artistName)
    {
        var response = await $"https://api.musixmatch.com/ws/1.1"
            .AppendPathSegment("artist.search")
            .SetQueryParam("apikey", ApiKey)
            .SetQueryParam("format", "json")
            .SetQueryParam("q_artist", artistName)
            .GetStringAsync();

        var artists = response.Fetch("message.body.artist_list").ToJArray();

        var artist = artists.First().FetchJToken("artist");

        return new ArtistInfo
        {
            Id = artist.Fetch("artist_id"),
            Name = artist.Fetch("artist_name"),
            Country = artist.Fetch("artist_country"),
            BeginYear = artist.Fetch("begin_date_year"),
            EndYear = artist.Fetch("end_date_year")
        };
    }

    private async Task<TrackInfo> GetTrackAsync(string trackName, string artistName)
    {
        var artist = await GetArtistInfoAsync(artistName);

        var response = await $"https://api.musixmatch.com/ws/1.1"
            .AppendPathSegment("track.search")
            .SetQueryParam("apikey", ApiKey)
            .SetQueryParam("format", "json")
            .SetQueryParam("q_track", trackName)
            .SetQueryParam("f_artist_id", artist.Id)
            .GetStringAsync();

        var tracks = response.Fetch("message.body.track_list").ToJArray();
        var track = tracks.First().FetchJToken("track");

        return new TrackInfo
        {
            Name = track.Fetch("track_name"),
            Id = track.Fetch("track_id"),
            AlbumId = track.Fetch("album_id"),
            AlbumName = track.Fetch("album_name"),
            ArtistId = track.Fetch("artist_id"),
            ArtistName = track.Fetch("artist_name"),
        };
    }
    
    private async Task<string> GetLyricAsync(string trackName, string artistName)
    {
        try
        {
            var response = await "http://api.chartlyrics.com/apiv1.asmx"
                .AppendPathSegment("SearchLyricDirect")
                .SetQueryParam("artist", artistName)
                .SetQueryParam("song", trackName)
                .GetStringAsync();

            var xml = XDocument.Load(new StringReader(response));

            var nodes = xml.DescendantNodes()
                .OfType<XElement>();

            if (nodes.Any(x => x.Name.LocalName == "Lyric" && x.Value.IsNotNullOrEmpty()))
            {
                var re = nodes.First(x => x.Name.LocalName == "Lyric")
                    .Value;

                return $"{re}\r\nhttp://www.chartlyrics.com/api.aspx";
            }

            return await GetLyricByMusixmatchAsync(artistName, trackName);
        }
        catch(InvalidOperationException e)
        {
            return await GetLyricByMusixmatchAsync(artistName, trackName);
        }

    }

    private async Task<string> GetLyricByMusixmatchAsync(string artistName, string trackName)
    {
        var track = await GetTrackAsync(trackName, artistName);

        var response1 = await $"https://api.musixmatch.com/ws/1.1"
            .AppendPathSegment("track.lyrics.get")
            .SetQueryParam("apikey", ApiKey)
            .SetQueryParam("format", "json")
            .SetQueryParam("track_id", track.Id)
            .GetStringAsync();

        return $"{response1.Fetch("message.body.lyrics.lyrics_body")}\r\n只有30%的歌词。\r\nhttps://api.musixmatch.com";
    }

    #endregion
    
}