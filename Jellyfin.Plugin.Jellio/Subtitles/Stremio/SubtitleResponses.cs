using System;
using System.Globalization;
using System.Linq;

namespace Jellyfin.Plugin.Jellio.Subtitles.Stremio;

internal static class SubtitleResponses
{
    public static SubtitlesResponseDto From(SubtitleOutcome outcome, string baseUrl, string authToken) => outcome switch
    {
        SubtitleOutcome.Offered offered => new(offered.Tracks.Select(track => ToDto(track, baseUrl, authToken)).ToList()),
        _ => new([]),
    };

    private static SubtitleDto ToDto(SubtitleTrack track, string baseUrl, string authToken)
    {
        var versionUrl = $"{baseUrl}/Videos/{track.ItemId}/{track.MediaSourceId}";
        var apiKey = $"ApiKey={Uri.EscapeDataString(authToken)}";

        return new(
            Id: $"jelliopp:{track.MediaSourceId}:{Invariant(track.StreamIndex)}",
            Url: $"{versionUrl}/Subtitles/{Invariant(track.StreamIndex)}/0/Stream.{track.Format.ToString().ToLowerInvariant()}?{apiKey}",
            Lang: track.Language,
            Label: track.Label,
            Fonts: track.FontAttachmentIndexes.Count == 0
                ? null
                : track.FontAttachmentIndexes.Select(index => $"{versionUrl}/Attachments/{Invariant(index)}?{apiKey}").ToList());
    }

    private static string Invariant(int value) => value.ToString(CultureInfo.InvariantCulture);
}
