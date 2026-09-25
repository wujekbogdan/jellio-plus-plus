using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Jellio.Subtitles.Stremio;

internal sealed record SubtitlesResponseDto([property: JsonPropertyName("subtitles")] IReadOnlyList<SubtitleDto> Subtitles);
