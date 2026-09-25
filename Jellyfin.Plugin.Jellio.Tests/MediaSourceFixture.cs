using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBrowser.Model.Dto;

namespace Jellyfin.Plugin.Jellio.Tests;

/// <summary>
/// Loads media source fixtures. Each fixture is a media source that Jellyfin reported for a real file, with machine-specific ids and paths replaced.
/// </summary>
internal static class MediaSourceFixture
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public static MediaSourceInfo Load(string slug)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "MediaSourceInfo", $"{slug}.json");
        return JsonSerializer.Deserialize<MediaSourceInfo>(File.ReadAllText(path), Options)!;
    }
}
