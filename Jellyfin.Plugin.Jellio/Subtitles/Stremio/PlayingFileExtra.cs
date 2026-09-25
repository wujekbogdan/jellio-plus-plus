using System;
using System.Globalization;
using System.Linq;

namespace Jellyfin.Plugin.Jellio.Subtitles.Stremio;

/// <summary>
/// Reads the playing file from the extra path segment of a request target. The target must be taken before percent-decoding, because a decoded <c>&amp;</c> or <c>=</c> in a value can't be told apart from a separator.
/// </summary>
internal static class PlayingFileExtra
{
    private const string JsonEnding = ".json";

    public static PlayingFile? FromRawTarget(string rawTarget)
    {
        var extra = rawTarget[(rawTarget.LastIndexOf('/') + 1)..^JsonEnding.Length]
            .Split('&')
            .Select(pair => pair.Split('=', 2))
            .Where(pair => pair.Length == 2)
            .ToDictionary(pair => pair[0], pair => Uri.UnescapeDataString(pair[1]));

        return extra.TryGetValue("filename", out var filename)
            && extra.TryGetValue("videoSize", out var videoSize)
            && long.TryParse(videoSize, NumberStyles.None, CultureInfo.InvariantCulture, out var size)
                ? new PlayingFile(filename, size)
                : null;
    }
}
