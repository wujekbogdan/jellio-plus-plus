using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Jellyfin.Plugin.Jellio.Configuration;

namespace Jellyfin.Plugin.Jellio.Models;

public record ConfigResponse(
    [property: JsonPropertyName("jellyseerrEnabled")] bool JellyseerrEnabled,
    [property: JsonPropertyName("jellyseerrUrl")] string JellyseerrUrl,
    [property: JsonPropertyName("jellyseerrApiKey")] string JellyseerrApiKey,
    [property: JsonPropertyName("publicBaseUrl")] string PublicBaseUrl,
    [property: JsonPropertyName("selectedLibraries")] IReadOnlyList<Guid> SelectedLibraries,
    [property: JsonPropertyName("catalogSortOrders")] IReadOnlyList<CatalogSortOrderDto> CatalogSortOrders)
{
    public static ConfigResponse From(PluginConfiguration config) =>
        new(
            config.JellyseerrEnabled,
            config.JellyseerrUrl,
            config.JellyseerrApiKey,
            config.PublicBaseUrl,
            config.SelectedLibraries,
            config.CatalogSortOrders.Select(stored => new CatalogSortOrderDto(stored.LibraryId, stored.SortOrder)).ToList());
}
