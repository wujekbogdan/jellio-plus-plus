using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

/// <summary>
/// Labels subtitle tracks the way Jellyfin builds <see cref="MediaStream.DisplayTitle"/>, without the language.
/// Tracks of one language with the same label get <c>#1</c>, <c>#2</c>, and so on. The numbers follow the order of the given tracks, so order the tracks first.
/// </summary>
internal static class SubtitleLabels
{
    private const string Separator = " - ";

    public static IReadOnlyDictionary<int, string> For(IReadOnlyList<MediaStream> tracks) =>
        tracks
            .GroupBy(SubtitleLanguages.Of, StringComparer.OrdinalIgnoreCase)
            .SelectMany(language => language.GroupBy(DisplayTitleWithoutLanguage).SelectMany(Numbered))
            .ToDictionary();

    private static IEnumerable<KeyValuePair<int, string>> Numbered(IGrouping<string, MediaStream> sameLabel) =>
        sameLabel.Count() == 1
            ? [KeyValuePair.Create(sameLabel.Single().Index, sameLabel.Key)]
            : sameLabel.Select((track, position) => KeyValuePair.Create(track.Index, $"{sameLabel.Key} #{position + 1}"));

    private static string DisplayTitleWithoutLanguage(MediaStream track)
    {
        string?[] tags =
        [
            track.IsHearingImpaired == true ? JellyfinWord(track.LocalizedHearingImpaired, "Hearing Impaired") : null,
            track.IsDefault ? JellyfinWord(track.LocalizedDefault, "Default") : null,
            track.IsForced ? JellyfinWord(track.LocalizedForced, "Forced") : null,
            track.Codec?.ToUpperInvariant(),
            track.IsExternal ? JellyfinWord(track.LocalizedExternal, "External") : null,
        ];
        var presentTags = tags.OfType<string>().Where(tag => tag.Length > 0);

        return string.IsNullOrEmpty(track.Title)
            ? string.Join(Separator, presentTags)
            : string.Join(Separator, presentTags.Where(tag => !track.Title.Contains(tag, StringComparison.OrdinalIgnoreCase)).Prepend(track.Title));
    }

    private static string JellyfinWord(string? localized, string fallback) => string.IsNullOrEmpty(localized) ? fallback : localized;
}
