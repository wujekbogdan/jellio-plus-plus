using System.Collections.Generic;
using System.Globalization;

namespace Jellyfin.Plugin.Jellio.Streams;

/// <summary>
/// The query parameters of the Jellyfin <c>master.m3u8</c> request for one stream entry.
/// </summary>
public static class HlsStreamQuery
{
    public static IReadOnlyList<KeyValuePair<string, string?>> For(EntryStreams entry, string mediaSourceId, string authToken)
    {
        KeyValuePair<string, string?>[] segmentContainer = SegmentContainerSelection.For(entry) == SegmentContainer.Fmp4
            ? [new("segmentContainer", "mp4")]
            : [];

        KeyValuePair<string, string?>[] audioStreamIndex = entry.AudioTrack is null
            ? []
            : [new("audioStreamIndex", entry.AudioTrack.StreamIndex.ToString(CultureInfo.InvariantCulture))];

        return
        [
            new("mediaSourceId", mediaSourceId),
            new("ApiKey", authToken),
            new("videoCodec", DeclaredCodecs.StremioPlayerVideo.QueryValue),
            new("audioCodec", DeclaredCodecs.StremioPlayerAudio.QueryValue),
            .. audioStreamIndex,
            .. segmentContainer,
        ];
    }
}
