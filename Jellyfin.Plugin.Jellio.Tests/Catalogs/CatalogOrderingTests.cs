using Jellyfin.Data.Enums;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Plugin.Jellio.Catalogs;
using Jellyfin.Plugin.Jellio.Models;

namespace Jellyfin.Plugin.Jellio.Tests.Catalogs;

public class CatalogOrderingTests
{
    [Theory]
    [InlineData(CatalogSortOrder.Name, StremioType.Movie, ItemSortBy.SortName, SortOrder.Ascending)]
    [InlineData(CatalogSortOrder.Name, StremioType.Series, ItemSortBy.SortName, SortOrder.Ascending)]
    [InlineData(CatalogSortOrder.RecentlyAdded, StremioType.Movie, ItemSortBy.DateCreated, SortOrder.Descending)]
    [InlineData(CatalogSortOrder.RecentlyAdded, StremioType.Series, ItemSortBy.DateLastContentAdded, SortOrder.Descending)]
    [InlineData(CatalogSortOrder.ReleaseDate, StremioType.Movie, ItemSortBy.PremiereDate, SortOrder.Descending)]
    [InlineData(CatalogSortOrder.ReleaseDate, StremioType.Series, ItemSortBy.PremiereDate, SortOrder.Descending)]
    public void SortsTheCatalogItemsByTheChosenOrder(
        CatalogSortOrder sortOrder,
        StremioType stremioType,
        ItemSortBy expectedSortBy,
        SortOrder expectedDirection)
    {
        Assert.Equal([(expectedSortBy, expectedDirection)], CatalogOrdering.For(sortOrder, stremioType));
    }
}
