using System.Linq;
using Jellyfin.Plugin.Jellio.Streams;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Tests.Streams;

public class HlsStreamQueryTests
{
    [Fact]
    public void For_Fmp4Entry_RequestsMp4Segments()
    {
        var entry = Assert.Single(EntryStreams.ForSource(SourceWith(Video("av1"), Audio("opus", index: 1))));

        var query = HlsStreamQuery.For(entry, mediaSourceId: "source-id", authToken: "token");

        Assert.Contains(new("segmentContainer", "mp4"), query);
    }

    [Fact]
    public void For_MpegTsEntryWithTrackChoice_OmitsTheSegmentContainer()
    {
        var entries = EntryStreams.ForSource(SourceWith(Video("h264"), Audio("ac3", index: 1), Audio("eac3", index: 2)));

        var query = HlsStreamQuery.For(entries[1], mediaSourceId: "source-id", authToken: "token");

        Assert.Equal(
            [
                ("mediaSourceId", "source-id"),
                ("ApiKey", "token"),
                ("videoCodec", "h264,hevc,av1"),
                ("audioCodec", "aac,mp3,ac3,eac3,flac,opus"),
                ("audioStreamIndex", "2"),
            ],
            query.Select(parameter => (parameter.Key, parameter.Value)));
    }

    private static MediaSourceInfo SourceWith(params MediaStream[] streams) => new() { MediaStreams = streams };

    private static MediaStream Video(string codec) => new() { Type = MediaStreamType.Video, Index = 0, Codec = codec };

    private static MediaStream Audio(string codec, int index) =>
        new() { Type = MediaStreamType.Audio, Index = index, Codec = codec, Channels = 2 };
}
