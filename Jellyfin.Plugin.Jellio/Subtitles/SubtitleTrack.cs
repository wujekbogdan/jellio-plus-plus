using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.Jellio.Subtitles;

/// <summary>
/// A subtitle track to offer, and the format to deliver it in. Only a styled track has font attachment indexes.
/// </summary>
internal sealed record SubtitleTrack(
    Guid ItemId,
    string MediaSourceId,
    int StreamIndex,
    string Language,
    SubtitleFormat Format,
    string Label,
    IReadOnlyList<int> FontAttachmentIndexes);
