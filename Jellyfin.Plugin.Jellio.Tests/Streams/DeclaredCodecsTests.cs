using Jellyfin.Plugin.Jellio.Streams;

namespace Jellyfin.Plugin.Jellio.Tests.Streams;

public class DeclaredCodecsTests
{
    [Fact]
    public void FallbackTranscodeTarget_IsTheFirstDeclaredCodec()
    {
        var codecs = new DeclaredCodecs(["opus", "aac"]);

        Assert.Equal("opus", codecs.FallbackTranscodeTarget);
    }

    [Fact]
    public void OutputCodecFor_DeclaredSourceCodec_KeepsTheSourceCodec()
    {
        var codecs = new DeclaredCodecs(["aac", "opus"]);

        Assert.Equal("opus", codecs.OutputCodecFor("OPUS"));
    }

    [Fact]
    public void OutputCodecFor_UndeclaredSourceCodec_ReturnsTheFallbackTranscodeTarget()
    {
        var codecs = new DeclaredCodecs(["aac", "opus"]);

        Assert.Equal("aac", codecs.OutputCodecFor("dts"));
    }

    [Fact]
    public void QueryValue_JoinsTheCodecsInDeclaredOrder()
    {
        var codecs = new DeclaredCodecs(["h264", "hevc", "av1"]);

        Assert.Equal("h264,hevc,av1", codecs.QueryValue);
    }

    // The segment container selection counts on both fallbacks to fit in MPEG-TS.
    [Fact]
    public void StremioPlayerCodecs_FallBackToH264AndAac()
    {
        Assert.Equal("h264", DeclaredCodecs.StremioPlayerVideo.FallbackTranscodeTarget);
        Assert.Equal("aac", DeclaredCodecs.StremioPlayerAudio.FallbackTranscodeTarget);
    }
}
