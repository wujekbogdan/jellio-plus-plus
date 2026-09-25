using System.Text;
using Jellyfin.Plugin.Jellio.Catalogs;
using Jellyfin.Plugin.Jellio.Helpers;
using Microsoft.AspNetCore.WebUtilities;

namespace Jellyfin.Plugin.Jellio.Tests.Helpers;

public class ConfigDecoderTests
{
    private const string LibraryGuid = "0c4e0d4a-7e1f-4b5c-9f3a-2d6b8e1a5c7f";

    [Fact]
    public void ConfigWithoutCatalogSortOrders_DecodesWithNone()
    {
        var config = ConfigDecoder.Decode(Encode($$"""{"ServerName":"Home","AuthToken":"token","LibrariesGuids":["{{LibraryGuid}}"]}"""));

        Assert.Empty(config!.CatalogSortOrders);
    }

    [Fact]
    public void CatalogSortOrders_DecodeFromTheirNames()
    {
        var config = ConfigDecoder.Decode(Encode($$$"""{"ServerName":"Home","AuthToken":"token","LibrariesGuids":["{{{LibraryGuid}}}"],"CatalogSortOrders":{"{{{LibraryGuid}}}":"RecentlyAdded"}}"""));

        Assert.Equal(new Dictionary<Guid, CatalogSortOrder> { [Guid.Parse(LibraryGuid)] = CatalogSortOrder.RecentlyAdded }, config!.CatalogSortOrders);
    }

    [Theory]
    [InlineData("\"Popularity\"")]
    [InlineData("1")]
    [InlineData("7")]
    public void UnknownCatalogSortOrder_IsNotAConfig(string sortOrder)
    {
        var config = ConfigDecoder.Decode(Encode($$$"""{"ServerName":"Home","AuthToken":"token","LibrariesGuids":["{{{LibraryGuid}}}"],"CatalogSortOrders":{"{{{LibraryGuid}}}":{{{sortOrder}}}}}"""));

        Assert.Null(config);
    }

    private static string Encode(string json) => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(json));
}
