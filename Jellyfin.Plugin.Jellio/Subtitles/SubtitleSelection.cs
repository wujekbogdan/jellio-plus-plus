using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

/// <summary>
/// Selects the subtitle tracks of a title to offer for the playing file. Only the tracks of the version that is the playing file are offered. A version is the playing file when its file name and size are equal to those of the playing file.
/// </summary>
internal static class SubtitleSelection
{
    private static readonly ISubtitleSource[] Sources = [new EmbeddedTextSubtitles(), new ExternalTextSubtitles()];

    public static SubtitleOutcome For(SubtitleRequest request)
    {
        if (request.Versions.Count == 0)
        {
            return new SubtitleOutcome.TitleNotInLibrary();
        }

        var playingVersion = request.PlayingFile is { } playingFile
            ? request.Versions.FirstOrDefault(version => IsPlaying(version, playingFile))
            : null;
        if (playingVersion is null)
        {
            return new SubtitleOutcome.NoMatchingVersion();
        }

        var deliverable = Sources
            .SelectMany(source => source.TracksIn(playingVersion.Source))
            .SelectMany(stream => SubtitleFormats.ToDeliver(stream.Codec) is { } format
                ? [new DeliverableStream(stream, format)]
                : Array.Empty<DeliverableStream>())
            .OrderBy(candidate => TrackRanks.Of(candidate.Stream))
            .ThenBy(candidate => candidate.Stream.Index)
            .ToList();
        if (deliverable.Count == 0)
        {
            return new SubtitleOutcome.NoUsableTracks(Skipped: playingVersion.Source.MediaStreams.Count(stream => stream.Type == MediaStreamType.Subtitle));
        }

        var labels = SubtitleLabels.For(deliverable.Select(candidate => candidate.Stream).ToList());
        var fonts = FontAttachments.In(playingVersion.Source);
        return new SubtitleOutcome.Offered(deliverable
            .Select(candidate => ToTrack(playingVersion, candidate, labels[candidate.Stream.Index], fonts))
            .ToList());
    }

    private static SubtitleTrack ToTrack(TitleVersion version, DeliverableStream candidate, string? label, IReadOnlyList<int> fonts) =>
        new(
            version.ItemId,
            version.Source.Id,
            candidate.Stream.Index,
            SubtitleLanguages.Of(candidate.Stream),
            candidate.Format,
            label,
            FontAttachmentIndexes: candidate.Format.IsStyled() ? fonts : []);

    private static bool IsPlaying(TitleVersion version, PlayingFile playingFile) =>
        version.Source.Size == playingFile.Size
        && string.Equals(Path.GetFileName(version.Source.Path), playingFile.Filename, StringComparison.Ordinal);

    private sealed record DeliverableStream(MediaStream Stream, SubtitleFormat Format);
}
