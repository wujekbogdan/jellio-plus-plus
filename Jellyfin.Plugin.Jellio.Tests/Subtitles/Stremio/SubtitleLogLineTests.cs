using Jellyfin.Plugin.Jellio.Subtitles;
using Jellyfin.Plugin.Jellio.Subtitles.Stremio;

namespace Jellyfin.Plugin.Jellio.Tests.Subtitles.Stremio;

public class SubtitleLogLineTests
{
    [Fact]
    public void EachOutcome_IsDescribedWithTheRequestedIdAndThePlayingFile()
    {
        var playingFile = new PlayingFile("The Matrix - 1080p.mkv", 146761);
        var track = new SubtitleTrack(Guid.NewGuid(), "a1b2c3", StreamIndex: 4, Language: "eng", SubtitleFormat.Srt, Label: null, FontAttachmentIndexes: []);
        string Line(PlayingFile? file, SubtitleOutcome outcome) => SubtitleLogLine.For("tt0133093", file, outcome);

        Assert.Equal("[Subtitles] tt0133093 · The Matrix - 1080p.mkv (146761 bytes) · title not in library", Line(playingFile, new SubtitleOutcome.TitleNotInLibrary()));
        Assert.Equal("[Subtitles] tt0133093 · The Matrix - 1080p.mkv (146761 bytes) · no matching version, 2 checked", Line(playingFile, new SubtitleOutcome.NoMatchingVersion(VersionCount: 2)));
        Assert.Equal("[Subtitles] tt0133093 · The Matrix - 1080p.mkv (146761 bytes) · no usable tracks, 3 skipped", Line(playingFile, new SubtitleOutcome.NoUsableTracks(Skipped: 3)));
        Assert.Equal("[Subtitles] tt0133093 · The Matrix - 1080p.mkv (146761 bytes) · 2 tracks offered", Line(playingFile, new SubtitleOutcome.Offered([track, track])));
        Assert.Equal("[Subtitles] tt0133093 · no file details · no matching version, 1 checked", Line(null, new SubtitleOutcome.NoMatchingVersion(VersionCount: 1)));
    }
}
