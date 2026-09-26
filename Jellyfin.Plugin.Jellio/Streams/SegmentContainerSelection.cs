using System;
using System.Collections.Generic;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Streams;

/// <summary>
/// Chooses the HLS segment container for the video and audio streams that Jellyfin writes, after it copies or transcodes them.
/// MPEG-TS is the default container of Jellyfin, and fMP4 is the container for codecs that MPEG-TS cannot carry.
/// </summary>
public static class SegmentContainerSelection
{
    // Only these codecs are safe in MPEG-TS, so a stream with any other codec gets fMP4.
    // The lists are the codecs that the ffmpeg MPEG-TS muxer (used by Jellyfin) maps to a standard stream type.
    // It muxes other codecs (AV1, FLAC, and Opus for most demuxers) as unidentified data, and the player skips that track.
    private static readonly HashSet<string> MpegTsVideoCodecs = new(["h264", "hevc"], StringComparer.OrdinalIgnoreCase);

    private static readonly HashSet<string> MpegTsAudioCodecs = new(["aac", "mp3", "ac3", "eac3"], StringComparer.OrdinalIgnoreCase);

    public static SegmentContainer For(EntryStreams entry) =>
        FitsMpegTs(entry.Video, DeclaredCodecs.StremioPlayerVideo, MpegTsVideoCodecs)
        && FitsMpegTs(entry.Audio, DeclaredCodecs.StremioPlayerAudio, MpegTsAudioCodecs)
            ? SegmentContainer.MpegTs
            : SegmentContainer.Fmp4;

    private static bool FitsMpegTs(MediaStream? stream, DeclaredCodecs declaredCodecs, HashSet<string> mpegTsCodecs) =>
        stream is null || mpegTsCodecs.Contains(declaredCodecs.OutputCodecFor(stream.Codec));
}
