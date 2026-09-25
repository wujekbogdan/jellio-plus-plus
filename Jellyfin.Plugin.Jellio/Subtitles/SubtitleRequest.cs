using System.Collections.Generic;

namespace Jellyfin.Plugin.Jellio.Subtitles;

internal sealed record SubtitleRequest(IReadOnlyList<TitleVersion> Versions, PlayingFile? PlayingFile);
