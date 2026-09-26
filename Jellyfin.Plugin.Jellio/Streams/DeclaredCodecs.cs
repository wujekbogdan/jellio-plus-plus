using System;
using System.Collections.Generic;
using System.Linq;

namespace Jellyfin.Plugin.Jellio.Streams;

/// <summary>
/// The codecs that a stream request declares to Jellyfin's HLS endpoint as playable.
/// Jellyfin copies a source stream with a declared codec and transcodes every other stream.
/// </summary>
public sealed class DeclaredCodecs(IReadOnlyList<string> codecs)
{
    /*
     * Jellyfin's HLS endpoint requires the caller to declare which codecs the player supports.
     * It compares these against the media file's codecs to decide whether to pass through without re-encoding or transcode.
     *
     * Stremio's addon protocol has no mechanism for the client to advertise its codec capabilities to addons, so we hardcode them here. The lists below reflect what Stremio's players can decode. This is the same pattern every Jellyfin client follows - e.g. jellyfin-web builds its codec list.
     * See: https://github.com/jellyfin/jellyfin-web/blob/285196329/src/scripts/browserDeviceProfile.js#L914-L925
     *
     * Without these params Jellyfin would fall back to "m3u8" as the audio codec name, producing invalid FFmpeg commands.
     * See: https://github.com/jellyfin/jellyfin/issues/12926
     */
    public static DeclaredCodecs StremioPlayerVideo { get; } = new(["h264", "hevc", "av1"]);

    public static DeclaredCodecs StremioPlayerAudio { get; } = new(["aac", "mp3", "ac3", "eac3", "flac", "opus"]);

    /// <summary>
    /// Gets the first declared codec, which Jellyfin transcodes to when it cannot copy a stream.
    /// </summary>
    public string FallbackTranscodeTarget { get; } = codecs[0];

    public string QueryValue { get; } = string.Join(',', codecs);

    public string OutputCodecFor(string sourceCodec) =>
        codecs.FirstOrDefault(codec => string.Equals(codec, sourceCodec, StringComparison.OrdinalIgnoreCase))
        ?? FallbackTranscodeTarget;
}
