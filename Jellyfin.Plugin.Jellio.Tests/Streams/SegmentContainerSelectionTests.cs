using System.Linq;
using Jellyfin.Plugin.Jellio.Streams;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Tests.Streams;

public class SegmentContainerSelectionTests
{
    [Fact]
    public void For_Av1VideoWithOpusAudio_ReturnsFmp4()
    {
        Assert.Equal(SegmentContainer.Fmp4, ContainerFor(Video("av1"), Audio("opus")));
    }

    [Fact]
    public void For_HevcVideoWithEac3Audio_ReturnsMpegTs()
    {
        Assert.Equal(SegmentContainer.MpegTs, ContainerFor(Video("hevc"), Audio("eac3")));
    }

    [Fact]
    public void For_UndeclaredAudioCodec_ReturnsMpegTs()
    {
        Assert.Equal(SegmentContainer.MpegTs, ContainerFor(Video("h264"), Audio("dts")));
    }

    [Fact]
    public void For_UndeclaredVideoCodec_ReturnsMpegTs()
    {
        Assert.Equal(SegmentContainer.MpegTs, ContainerFor(Video("vp9"), Audio("aac")));
    }

    [Fact]
    public void For_H264VideoWithOpusAudio_ReturnsFmp4()
    {
        Assert.Equal(SegmentContainer.Fmp4, ContainerFor(Video("h264"), Audio("opus")));
    }

    [Fact]
    public void For_NoAudio_ChoosesByVideoOnly()
    {
        Assert.Equal(SegmentContainer.MpegTs, ContainerFor(Video("h264")));
    }

    [Fact]
    public void For_NoVideo_ChoosesByAudioOnly()
    {
        Assert.Equal(SegmentContainer.Fmp4, ContainerFor(Audio("flac")));
    }

    [Fact]
    public void For_SeveralAudioTracks_ChoosesPerEntry()
    {
        var source = new MediaSourceInfo { MediaStreams = [Video("h264"), Audio("ac3", index: 1), Audio("opus", index: 2)] };

        var containers = EntryStreams.ForSource(source).Select(SegmentContainerSelection.For);

        Assert.Equal([SegmentContainer.MpegTs, SegmentContainer.Fmp4], containers);
    }

    private static SegmentContainer ContainerFor(params MediaStream[] streams) =>
        SegmentContainerSelection.For(Assert.Single(EntryStreams.ForSource(new MediaSourceInfo { MediaStreams = streams })));

    private static MediaStream Video(string codec) => new() { Type = MediaStreamType.Video, Index = 0, Codec = codec };

    private static MediaStream Audio(string codec, int index = 1) =>
        new() { Type = MediaStreamType.Audio, Index = index, Codec = codec, Channels = 2 };
}
