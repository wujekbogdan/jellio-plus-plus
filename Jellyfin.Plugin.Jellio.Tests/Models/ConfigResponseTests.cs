using System.Text.Json;
using Jellyfin.Extensions.Json;
using Jellyfin.Plugin.Jellio.Catalogs;
using Jellyfin.Plugin.Jellio.Configuration;
using Jellyfin.Plugin.Jellio.Models;

namespace Jellyfin.Plugin.Jellio.Tests.Models;

public class ConfigResponseTests
{
    [Fact]
    public void StoredConfig_IsWrittenInTheShapeTheConfigPageReads()
    {
        var libraryId = Guid.Parse("0c4e0d4a-7e1f-4b5c-9f3a-2d6b8e1a5c7f");
        var config = new PluginConfiguration
        {
            JellyseerrEnabled = true,
            JellyseerrUrl = "https://jellyseerr.example",
            JellyseerrApiKey = "key",
            PublicBaseUrl = "https://jellyfin.example",
            SelectedLibraries = [libraryId],
            CatalogSortOrders = { new LibraryCatalogSortOrder { LibraryId = libraryId, SortOrder = CatalogSortOrder.RecentlyAdded } },
        };

        var responseJson = JsonSerializer.Serialize(ConfigResponse.From(config), JsonDefaults.PascalCaseOptions);

        Assert.Equal(
            """{"jellyseerrEnabled":true,"jellyseerrUrl":"https://jellyseerr.example","jellyseerrApiKey":"key","publicBaseUrl":"https://jellyfin.example","selectedLibraries":["0c4e0d4a7e1f4b5c9f3a2d6b8e1a5c7f"],"catalogSortOrders":[{"libraryId":"0c4e0d4a7e1f4b5c9f3a2d6b8e1a5c7f","sortOrder":"RecentlyAdded"}]}""",
            responseJson);
    }
}
