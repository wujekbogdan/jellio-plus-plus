using System.Xml.Serialization;
using Jellyfin.Plugin.Jellio.Catalogs;
using Jellyfin.Plugin.Jellio.Configuration;

namespace Jellyfin.Plugin.Jellio.Tests.Configuration;

public class PluginConfigurationTests
{
    private readonly XmlSerializer _serializer = new(typeof(PluginConfiguration));

    [Fact]
    public void StoredConfigWithoutCatalogSortOrders_LoadsWithNone()
    {
        const string storedXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <PluginConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
              <JellyseerrEnabled>false</JellyseerrEnabled>
              <PublicBaseUrl />
              <SelectedLibraries>
                <guid>0c4e0d4a-7e1f-4b5c-9f3a-2d6b8e1a5c7f</guid>
              </SelectedLibraries>
            </PluginConfiguration>
            """;

        var config = (PluginConfiguration)_serializer.Deserialize(new StringReader(storedXml))!;

        Assert.Empty(config.CatalogSortOrders);
    }

    [Fact]
    public void CatalogSortOrders_SurviveSaveAndLoad()
    {
        var libraryId = Guid.NewGuid();
        var config = new PluginConfiguration
        {
            CatalogSortOrders = { new LibraryCatalogSortOrder { LibraryId = libraryId, SortOrder = CatalogSortOrder.ReleaseDate } },
        };
        var xml = new StringWriter();
        _serializer.Serialize(xml, config);

        var loaded = (PluginConfiguration)_serializer.Deserialize(new StringReader(xml.ToString()))!;

        var stored = Assert.Single(loaded.CatalogSortOrders);
        Assert.Equal((libraryId, CatalogSortOrder.ReleaseDate), (stored.LibraryId, stored.SortOrder));
    }
}
