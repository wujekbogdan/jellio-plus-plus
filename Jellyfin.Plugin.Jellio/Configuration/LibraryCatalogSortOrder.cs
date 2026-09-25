using System;
using Jellyfin.Plugin.Jellio.Catalogs;

namespace Jellyfin.Plugin.Jellio.Configuration;

public class LibraryCatalogSortOrder
{
    public Guid LibraryId { get; set; }

    public CatalogSortOrder SortOrder { get; set; }
}
