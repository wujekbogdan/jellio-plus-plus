using System.Text.Json;
using Jellyfin.Extensions.Json;
using Jellyfin.Plugin.Jellio.Catalogs;
using Jellyfin.Plugin.Jellio.Models;

namespace Jellyfin.Plugin.Jellio.Tests.Models;

public class SaveConfigRequestTests
{
    private static readonly JsonSerializerOptions JellyfinRequestJson = new(JsonDefaults.PascalCaseOptions) { PropertyNameCaseInsensitive = true };

    [Fact]
    public void RequestFromTheOldConfigPage_StoresWhatItStoredBefore()
    {
        const string requestJson = """
            {"jellyseerrEnabled":true,"jellyseerrUrl":null,"jellyseerrApiKey":"key","selectedLibraries":["0c4e0d4a7e1f4b5c9f3a2d6b8e1a5c7f","not-a-guid"]}
            """;

        var config = Read(requestJson).ToConfiguration();

        Assert.True(config.JellyseerrEnabled);
        Assert.Equal((string.Empty, "key", string.Empty), (config.JellyseerrUrl, config.JellyseerrApiKey, config.PublicBaseUrl));
        Assert.Equal([Guid.Parse("0c4e0d4a-7e1f-4b5c-9f3a-2d6b8e1a5c7f")], config.SelectedLibraries);
        Assert.Empty(config.CatalogSortOrders);
    }

    [Fact]
    public void RequestWithoutLibraries_StoresNone()
    {
        Assert.Empty(Read("""{"jellyseerrEnabled":false}""").ToConfiguration().SelectedLibraries);
    }

    [Fact]
    public void CatalogSortOrders_AreStoredForTheirLibraries()
    {
        const string requestJson = """
            {"catalogSortOrders":[{"libraryId":"0c4e0d4a-7e1f-4b5c-9f3a-2d6b8e1a5c7f","sortOrder":"RecentlyAdded"},{"libraryId":"9b2f6c1e3d4a4e5f8a7b6c5d4e3f2a1b","sortOrder":"ReleaseDate"}]}
            """;

        var stored = Read(requestJson).ToConfiguration().CatalogSortOrders.Select(entry => (entry.LibraryId, entry.SortOrder));

        Assert.Equal(
            [
                (Guid.Parse("0c4e0d4a-7e1f-4b5c-9f3a-2d6b8e1a5c7f"), CatalogSortOrder.RecentlyAdded),
                (Guid.Parse("9b2f6c1e-3d4a-4e5f-8a7b-6c5d4e3f2a1b"), CatalogSortOrder.ReleaseDate),
            ],
            stored);
    }

    private static SaveConfigRequest Read(string requestJson) => JsonSerializer.Deserialize<SaveConfigRequest>(requestJson, JellyfinRequestJson)!;
}
