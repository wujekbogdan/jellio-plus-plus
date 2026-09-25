using System;
using System.Collections.Generic;
using Jellyfin.Data.Enums;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Plugin.Jellio.Models;

namespace Jellyfin.Plugin.Jellio.Catalogs;

internal static class CatalogOrdering
{
    public static IReadOnlyList<(ItemSortBy OrderBy, SortOrder SortOrder)> For(
        CatalogSortOrder sortOrder,
        StremioType stremioType) =>
        (sortOrder, stremioType) switch
        {
            (CatalogSortOrder.Name, _) => [(ItemSortBy.SortName, SortOrder.Ascending)],
            (CatalogSortOrder.RecentlyAdded, StremioType.Movie) => [(ItemSortBy.DateCreated, SortOrder.Descending)],
            (CatalogSortOrder.RecentlyAdded, StremioType.Series) => [(ItemSortBy.DateLastContentAdded, SortOrder.Descending)],
            (CatalogSortOrder.ReleaseDate, _) => [(ItemSortBy.PremiereDate, SortOrder.Descending)],
            _ => throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null),
        };
}
