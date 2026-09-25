namespace Jellyfin.Plugin.Jellio.Subtitles;

/// <summary>
/// Gives the format to deliver a track in, or <c>null</c> when the track is not delivered. A track is delivered only in its own format.
/// </summary>
internal static class SubtitleFormats
{
    public static SubtitleFormat? ToDeliver(string? codec) => codec?.ToLowerInvariant() switch
    {
        "subrip" or "srt" => SubtitleFormat.Srt,
        "webvtt" or "vtt" => SubtitleFormat.Vtt,
        "ass" => SubtitleFormat.Ass,
        "ssa" => SubtitleFormat.Ssa,
        _ => null,
    };

    public static bool IsStyled(this SubtitleFormat format) => format is SubtitleFormat.Ass or SubtitleFormat.Ssa;
}
