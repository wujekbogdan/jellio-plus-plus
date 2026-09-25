using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Jellio.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public bool JellyseerrEnabled { get; set; } = false;
    public string JellyseerrUrl { get; set; } = string.Empty;
    public string JellyseerrApiKey { get; set; } = string.Empty;
    public string PublicBaseUrl { get; set; } = string.Empty;
    public List<Guid> SelectedLibraries { get; set; } = new();
    public Collection<LibraryCatalogSortOrder> CatalogSortOrders { get; init; } = [];
}
