using System.Collections.Generic;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

internal interface ISubtitleSource
{
    IEnumerable<MediaStream> TracksIn(MediaSourceInfo source);
}
