using System;
using System.Text.Json.Serialization;
using Jellyfin.Plugin.Jellio.Catalogs;

namespace Jellyfin.Plugin.Jellio.Models;

public record CatalogSortOrderDto(
    [property: JsonPropertyName("libraryId")] Guid LibraryId,
    [property: JsonPropertyName("sortOrder")] CatalogSortOrder SortOrder);
