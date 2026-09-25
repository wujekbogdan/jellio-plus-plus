using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

internal static class SubtitleLanguages
{
    private const string Undetermined = "und";

    public static string Of(MediaStream track) => string.IsNullOrWhiteSpace(track.Language) ? Undetermined : track.Language;
}
