using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Streams;

/// <summary>
/// The source streams that one stream entry plays.
/// A source with an audio track choice gets one entry per track.
/// </summary>
public sealed class EntryStreams
{
    private EntryStreams(MediaStream? video, MediaStream? audio, AudioTrackChoice? audioTrack)
    {
        Video = video;
        Audio = audio;
        AudioTrack = audioTrack;
    }

    public MediaStream? Video { get; }

    public MediaStream? Audio { get; }

    public AudioTrackChoice? AudioTrack { get; }

    public static IReadOnlyList<EntryStreams> ForSource(MediaSourceInfo source)
    {
        var streams = source.MediaStreams ?? [];
        var video = streams.FirstOrDefault(stream => stream.Type == MediaStreamType.Video);
        var audioTracks = AudioTrackSelection.ForSource(source);

        return audioTracks.Count == 0
            ? [new(video, audio: streams.FirstOrDefault(stream => stream.Type == MediaStreamType.Audio), audioTrack: null)]
            : audioTracks
                .Select(audioTrack => new EntryStreams(
                    video,
                    audio: streams.Single(stream => stream.Type == MediaStreamType.Audio && stream.Index == audioTrack.StreamIndex),
                    audioTrack))
                .ToList();
    }
}
