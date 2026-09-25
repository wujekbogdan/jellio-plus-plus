using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.Jellio.Configuration;

namespace Jellyfin.Plugin.Jellio.Models;

public class SaveConfigRequest
{
    public bool JellyseerrEnabled { get; set; }
    public string? JellyseerrUrl { get; set; }
    public string? JellyseerrApiKey { get; set; }
    public string? PublicBaseUrl { get; set; }
    public List<string>? SelectedLibraries { get; set; }
    public IReadOnlyList<CatalogSortOrderDto>? CatalogSortOrders { get; init; }

    public PluginConfiguration ToConfiguration() =>
        new()
        {
            JellyseerrEnabled = JellyseerrEnabled,
            JellyseerrUrl = JellyseerrUrl ?? string.Empty,
            JellyseerrApiKey = JellyseerrApiKey ?? string.Empty,
            PublicBaseUrl = PublicBaseUrl ?? string.Empty,
            SelectedLibraries = (SelectedLibraries ?? [])
                .Where(id => Guid.TryParse(id, out _))
                .Select(Guid.Parse)
                .ToList(),
            CatalogSortOrders = new((CatalogSortOrders ?? [])
                .Select(entry => new LibraryCatalogSortOrder { LibraryId = entry.LibraryId, SortOrder = entry.SortOrder })
                .ToList()),
        };
}
