using Jellyfin.Plugin.Jellio.Streams;

namespace Jellyfin.Plugin.Jellio.Tests.Streams;

public class SegmentContainerSelectionSnapshotTests
{
    [Fact]
    public void For_Av1VideoWithOpusAudio_ReturnsFmp4()
    {
        var entry = Assert.Single(EntryStreams.ForSource(MediaSourceFixture.Load("av1-opus")));

        Assert.Equal(SegmentContainer.Fmp4, SegmentContainerSelection.For(entry));
    }
}
