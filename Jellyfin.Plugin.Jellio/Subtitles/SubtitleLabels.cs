using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

/// <summary>
/// Labels the subtitle tracks that share a language, so that a user can tell them apart.
/// A track that is the only one in its language gets no label.
/// A label holds the track title and its forced and hearing-impaired flags. Tracks that still look the same get <c>#1</c>, <c>#2</c>, and so on.
/// The numbers follow the order of the given tracks, so order the tracks first.
/// </summary>
internal static class SubtitleLabels
{
    private const string Separator = " · ";
    private const string Forced = "Forced";
    private const string HearingImpaired = "SDH";

    public static IReadOnlyDictionary<int, string?> For(IReadOnlyList<MediaStream> tracks) =>
        tracks
            .GroupBy(SubtitleLanguages.Of, StringComparer.OrdinalIgnoreCase)
            .SelectMany(language => language.Count() == 1
                ? [KeyValuePair.Create(language.Single().Index, (string?)null)]
                : language.GroupBy(Details).SelectMany(Numbered))
            .ToDictionary();

    private static IEnumerable<KeyValuePair<int, string?>> Numbered(IGrouping<string, MediaStream> sameDetails) =>
        sameDetails.Count() == 1
            ? [KeyValuePair.Create(sameDetails.Single().Index, NullIfEmpty(sameDetails.Key))]
            : sameDetails.Select((track, position) => KeyValuePair.Create(
                track.Index,
                (string?)string.Join(" ", new[] { sameDetails.Key, $"#{position + 1}" }.Where(part => part.Length > 0))));

    private static string Details(MediaStream track) =>
        string.Join(
            Separator,
            new[] { track.Title, track.IsForced ? Forced : null, track.IsHearingImpaired ? HearingImpaired : null }
                .Where(part => !string.IsNullOrEmpty(part))
                .Distinct(StringComparer.OrdinalIgnoreCase));

    private static string? NullIfEmpty(string value) => value.Length == 0 ? null : value;
}
