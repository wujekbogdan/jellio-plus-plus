using Jellyfin.Plugin.Jellio.Subtitles;
using Jellyfin.Plugin.Jellio.Subtitles.Stremio;

namespace Jellyfin.Plugin.Jellio.Tests.Subtitles.Stremio;

public class PlayingFileExtraTests
{
    [Fact]
    public void ExtraWithFileDetails_GivesThePlayingFile()
    {
        var playingFile = PlayingFileExtra.FromRawTarget(
            "/jelliopp/eyJhIjoiYiJ9/subtitles/movie/tt0133093/videoHash=8e245d9679d31e12&videoSize=146761&filename=subtitles-sample-720p.mkv.json");

        Assert.Equal(new PlayingFile("subtitles-sample-720p.mkv", 146761), playingFile);
    }

    [Fact]
    public void EncodedFilename_IsDecodedAfterTheSplit()
    {
        var playingFile = PlayingFileExtra.FromRawTarget(
            "/jelliopp/eyJhIjoiYiJ9/subtitles/movie/tt0133093/videoSize=146761&filename=Tom%20%26%20Jerry%20%3D%20Fun%20(%C5%81%C3%B3d%C5%BA).mkv.json");

        Assert.Equal(new PlayingFile("Tom & Jerry = Fun (Łódź).mkv", 146761), playingFile);
    }

    [Theory]
    [InlineData("/jelliopp/eyJhIjoiYiJ9/subtitles/movie/tt0133093/videoHash=8e245d9679d31e12&videoSize=146761.json")]
    [InlineData("/jelliopp/eyJhIjoiYiJ9/subtitles/movie/tt0133093/videoHash=8e245d9679d31e12&filename=subtitles-sample-720p.mkv.json")]
    [InlineData("/jelliopp/eyJhIjoiYiJ9/subtitles/movie/tt0133093/videoSize=large&filename=subtitles-sample-720p.mkv.json")]
    [InlineData("/jelliopp/eyJhIjoiYiJ9/subtitles/movie/tt0133093.json")]
    public void IncompleteFileDetails_GiveNoPlayingFile(string rawTarget)
    {
        Assert.Null(PlayingFileExtra.FromRawTarget(rawTarget));
    }
}
