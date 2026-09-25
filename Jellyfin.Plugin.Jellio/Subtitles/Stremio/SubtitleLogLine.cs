using System;
using System.Globalization;

namespace Jellyfin.Plugin.Jellio.Subtitles.Stremio;

internal static class SubtitleLogLine
{
    private const string Separator = " · ";

    public static string For(string requestedId, PlayingFile? playingFile, SubtitleOutcome outcome) =>
        string.Join(Separator, $"[Subtitles] {requestedId}", Describe(playingFile), Describe(outcome));

    private static string Describe(PlayingFile? playingFile) =>
        playingFile is null
            ? "no file details"
            : string.Create(CultureInfo.InvariantCulture, $"{playingFile.Filename} ({playingFile.Size} bytes)");

    private static string Describe(SubtitleOutcome outcome) => outcome switch
    {
        SubtitleOutcome.TitleNotInLibrary => "title not in library",
        SubtitleOutcome.NoMatchingVersion noMatch => string.Create(CultureInfo.InvariantCulture, $"no matching version, {noMatch.VersionCount} checked"),
        SubtitleOutcome.NoUsableTracks noUsable => string.Create(CultureInfo.InvariantCulture, $"no usable tracks, {noUsable.Skipped} skipped"),
        SubtitleOutcome.Offered offered => string.Create(CultureInfo.InvariantCulture, $"{offered.Tracks.Count} tracks offered"),
        _ => throw new ArgumentOutOfRangeException(nameof(outcome)),
    };
}
