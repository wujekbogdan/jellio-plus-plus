using Jellyfin.Plugin.Jellio.Streams;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Tests.Streams;

public class EntryStreamsTests
{
    [Fact]
    public void ForSource_SingleAudioTrack_ReturnsOneEntryWithoutTrackChoice()
    {
        var video = Video(index: 0);
        var audio = Audio(index: 1);

        var entry = Assert.Single(EntryStreams.ForSource(SourceWith(video, audio)));

        Assert.Same(video, entry.Video);
        Assert.Same(audio, entry.Audio);
        Assert.Null(entry.AudioTrack);
    }

    [Fact]
    public void ForSource_SeveralAudioTracks_ReturnsOneEntryPerTrack()
    {
        var english = Audio(index: 1, codec: "eac3");
        var japanese = Audio(index: 3, codec: "opus");

        var entries = EntryStreams.ForSource(SourceWith(Video(index: 0), english, japanese));

        Assert.Collection(
            entries,
            first =>
            {
                Assert.Same(english, first.Audio);
                Assert.Equal(1, first.AudioTrack?.StreamIndex);
            },
            second =>
            {
                Assert.Same(japanese, second.Audio);
                Assert.Equal(3, second.AudioTrack?.StreamIndex);
            });
    }

    private static MediaSourceInfo SourceWith(params MediaStream[] streams) => new() { MediaStreams = streams };

    private static MediaStream Video(int index) => new() { Type = MediaStreamType.Video, Index = index, Codec = "h264" };

    private static MediaStream Audio(int index, string codec = "aac", int channels = 2) =>
        new() { Type = MediaStreamType.Audio, Index = index, Codec = codec, Channels = channels };
}
