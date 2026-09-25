using System;
using System.Collections.Generic;
using Jellyfin.Plugin.Jellio.Catalogs;

namespace Jellyfin.Plugin.Jellio.Models;

public class ConfigModel
{
    public required string ServerName { get; init; }
    public required string AuthToken { get; init; }

    public required IReadOnlyList<Guid> LibrariesGuids { get; init; }

    public IReadOnlyDictionary<Guid, CatalogSortOrder> CatalogSortOrders { get; init; } = new Dictionary<Guid, CatalogSortOrder>();

    // Jellyseerr integration
    public bool JellyseerrEnabled { get; init; } = false;
    public string? JellyseerrUrl { get; init; }
    public string? JellyseerrApiKey { get; init; }

    // Optional: public base URL to build links (e.g., https://jellyfin.example.com)
    // Useful when Jellyfin runs behind a reverse proxy / tunnel and Request.Scheme is http
    public string? PublicBaseUrl { get; init; }
}
