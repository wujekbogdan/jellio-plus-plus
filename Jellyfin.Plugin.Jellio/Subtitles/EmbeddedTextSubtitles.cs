using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

internal sealed class EmbeddedTextSubtitles : ISubtitleSource
{
    public IEnumerable<MediaStream> TracksIn(MediaSourceInfo source) =>
        source.MediaStreams.Where(stream => stream.Type == MediaStreamType.Subtitle && !stream.IsExternal && stream.IsTextSubtitleStream);
}
