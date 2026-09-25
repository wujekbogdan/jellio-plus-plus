using Jellyfin.Plugin.Jellio.Subtitles;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Tests.Subtitles;

public class SubtitleSelectionTests
{
    [Fact]
    public void TextTrackInsideThePlayingVersion_IsOffered()
    {
        var version = new TitleVersion(Guid.NewGuid(), MediaSourceFixture.Load("subtitles-sample-720p"));

        var outcome = SubtitleSelection.For(new SubtitleRequest([version], new PlayingFile("subtitles-sample-720p.mkv", 146761)));

        var track = Assert.Single(Assert.IsType<SubtitleOutcome.Offered>(outcome).Tracks);
        Assert.Equivalent(
            new SubtitleTrack(version.ItemId, version.Source.Id, StreamIndex: 1, Language: "eng", SubtitleFormat.Srt, Label: null, FontAttachmentIndexes: []),
            track);
    }

    [Fact]
    public void VersionWithOnlyPictureTracks_HasNoUsableTracks()
    {
        var version = new TitleVersion(Guid.NewGuid(), MediaSourceFixture.Load("picture-subtitles"));

        var outcome = SubtitleSelection.For(new SubtitleRequest([version], new PlayingFile("picture-subtitles.mkv", version.Source.Size!.Value)));

        Assert.Equal(new SubtitleOutcome.NoUsableTracks(Skipped: 3), outcome);
    }

    [Fact]
    public void TwoVersions_OnlyThePlayingVersionIsOffered()
    {
        var fullHd = new TitleVersion(Guid.NewGuid(), MediaSourceFixture.Load("subtitles-sample-1080p"));
        var hd = new TitleVersion(Guid.NewGuid(), MediaSourceFixture.Load("subtitles-sample-720p"));
        SubtitleOutcome Playing(PlayingFile? file) => SubtitleSelection.For(new SubtitleRequest([fullHd, hd], file));

        var offered = Assert.IsType<SubtitleOutcome.Offered>(Playing(new PlayingFile("subtitles-sample-720p.mkv", hd.Source.Size!.Value)));

        Assert.All(offered.Tracks, track => Assert.Equal(hd.Source.Id, track.MediaSourceId));
        Assert.Equal(new SubtitleOutcome.NoMatchingVersion(VersionCount: 2), Playing(new PlayingFile("subtitles-sample-720p.mkv", fullHd.Source.Size!.Value)));
        Assert.Equal(new SubtitleOutcome.NoMatchingVersion(VersionCount: 2), Playing(new PlayingFile("another-release.mkv", hd.Source.Size!.Value)));
        Assert.Equal(new SubtitleOutcome.NoMatchingVersion(VersionCount: 2), Playing(null));
    }

    [Fact]
    public void EveryTextTrackAndTextFile_IsOfferedInItsOwnFormat()
    {
        var offered = Assert.IsType<SubtitleOutcome.Offered>(OfferedFor("subtitles-sample-1080p"));

        Assert.Equal(
            new Dictionary<int, SubtitleFormat>
            {
                [0] = SubtitleFormat.Srt, // the external .eng.srt file
                [2] = SubtitleFormat.Srt,
                [3] = SubtitleFormat.Srt,
                [4] = SubtitleFormat.Srt,
                [5] = SubtitleFormat.Ass,
                [6] = SubtitleFormat.Srt,
                [7] = SubtitleFormat.Vtt,
                [8] = SubtitleFormat.Srt,
                [9] = SubtitleFormat.Srt,
            },
            offered.Tracks.ToDictionary(track => track.StreamIndex, track => track.Format));
    }

    [Fact]
    public void SsaTrack_IsDeliveredAsSsa()
    {
        var source = MediaSourceFixture.Load("subtitles-sample-720p");
        source.MediaStreams.Single(stream => stream.Type == MediaStreamType.Subtitle).Codec = "ssa";

        var offered = Assert.IsType<SubtitleOutcome.Offered>(OfferedFor(source));

        Assert.Equal(SubtitleFormat.Ssa, Assert.Single(offered.Tracks).Format);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TrackWithoutALanguage_IsOfferedAsUndetermined(string? language)
    {
        var source = MediaSourceFixture.Load("subtitles-sample-720p");
        source.MediaStreams.Single(stream => stream.Type == MediaStreamType.Subtitle).Language = language;

        var offered = Assert.IsType<SubtitleOutcome.Offered>(OfferedFor(source));

        Assert.Equal("und", Assert.Single(offered.Tracks).Language);
    }

    [Fact]
    public void TracksWithoutALanguage_ShareOneLanguageAndAreLabelled()
    {
        var source = MediaSourceFixture.Load("subtitles-sample-720p");
        var track = source.MediaStreams.Single(stream => stream.Type == MediaStreamType.Subtitle);
        track.Language = null;
        source.MediaStreams = [.. source.MediaStreams, new MediaStream { Type = MediaStreamType.Subtitle, Index = 9, Codec = "subrip", Language = "" }];

        var offered = Assert.IsType<SubtitleOutcome.Offered>(OfferedFor(source));

        Assert.Equal(["#1", "#2"], offered.Tracks.Select(offeredTrack => offeredTrack.Label));
    }

    [Theory]
    [InlineData("mov_text")]
    [InlineData(null)]
    public void TrackOutsideTheDeliveredFormats_IsSkipped(string? codec)
    {
        var source = MediaSourceFixture.Load("subtitles-sample-720p");
        var track = source.MediaStreams.Single(stream => stream.Type == MediaStreamType.Subtitle);
        track.Codec = codec;
        track.IsExternal = true;

        Assert.Equal(new SubtitleOutcome.NoUsableTracks(Skipped: 1), OfferedFor(source));
    }

    [Fact]
    public void TracksThatShareALanguage_AreLabelledByTheirDetails()
    {
        var offered = Assert.IsType<SubtitleOutcome.Offered>(OfferedFor("subtitles-sample-1080p"));

        Assert.Equal(
            new Dictionary<int, string?>
            {
                [0] = "#2", // the external file
                [2] = "#1", // the default track
                [3] = "Forced",
                [4] = "SDH",
                [5] = "Styled",
                [6] = "#1",
                [7] = null,
                [8] = "#2",
                [9] = "Commentary",
            },
            offered.Tracks.ToDictionary(track => track.StreamIndex, track => track.Label));
    }

    [Fact]
    public void TracksOfALanguage_AreOrderedDefaultNormalHearingImpairedForcedTitled()
    {
        var offered = Assert.IsType<SubtitleOutcome.Offered>(OfferedFor("subtitles-sample-1080p"));

        Assert.Equal([2, 0, 4, 3, 5, 9], offered.Tracks.Where(track => track.Language == "eng").Select(track => track.StreamIndex));
    }

    [Fact]
    public void StyledTrack_CarriesTheFontsOfItsVersion()
    {
        var source = MediaSourceFixture.Load("subtitles-sample-1080p");
        source.MediaAttachments =
        [
            .. source.MediaAttachments,
            new MediaAttachment { Index = 10, FileName = "cover.jpg", MimeType = "image/jpeg" },
            new MediaAttachment { Index = 11, FileName = "legacy-sfnt", MimeType = "application/font-sfnt" },
            new MediaAttachment { Index = 12, FileName = "legacy-woff", MimeType = "application/font-woff" },
            new MediaAttachment { Index = 13, FileName = "COLLECTION.TTC" },
        ];

        var offered = Assert.IsType<SubtitleOutcome.Offered>(OfferedFor(source));

        Assert.Equal([9, 11, 12, 13], offered.Tracks.Single(track => track.Format == SubtitleFormat.Ass).FontAttachmentIndexes);
        Assert.All(offered.Tracks.Where(track => track.Format != SubtitleFormat.Ass), track => Assert.Empty(track.FontAttachmentIndexes));
    }

    [Fact]
    public void TitleWithoutVersions_IsNotInTheLibrary()
    {
        var outcome = SubtitleSelection.For(new SubtitleRequest([], new PlayingFile("another-release.mkv", 1000)));

        Assert.Equal(new SubtitleOutcome.TitleNotInLibrary(), outcome);
    }

    private static SubtitleOutcome OfferedFor(string fixture) => OfferedFor(MediaSourceFixture.Load(fixture));

    private static SubtitleOutcome OfferedFor(MediaSourceInfo source)
    {
        return SubtitleSelection.For(new SubtitleRequest(
            [new TitleVersion(Guid.NewGuid(), source)],
            new PlayingFile(Path.GetFileName(source.Path), source.Size!.Value)));
    }
}
