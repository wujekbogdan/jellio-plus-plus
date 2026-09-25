using System.Collections.Generic;
using System.Linq;
using Jellyfin.Data.Enums;
using Jellyfin.Database.Implementations.Entities;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Library;

public sealed class ItemResolver(ILibraryManager libraryManager)
{
    public IReadOnlyList<BaseItem> Resolve(User user, LibraryItemId id)
    {
        var item = libraryManager.GetItemById<BaseItem>(id.Value, user.Id);
        return item is null ? [] : [item];
    }

    public IReadOnlyList<BaseItem> Resolve(User user, ImdbMovieId id) =>
        libraryManager.GetItemList(new InternalItemsQuery(user)
        {
            HasAnyProviderId = new Dictionary<string, string> { [MetadataProvider.Imdb.ToString()] = id.Value },
            IncludeItemTypes = [BaseItemKind.Movie],
        });

    public IReadOnlyList<BaseItem> Resolve(User user, ImdbEpisodeId id)
    {
        var seriesIds = libraryManager
            .GetItemList(new InternalItemsQuery(user)
            {
                IncludeItemTypes = [BaseItemKind.Series],
                HasAnyProviderId = new Dictionary<string, string> { [MetadataProvider.Imdb.ToString()] = id.SeriesImdbId },
            })
            .Select(series => series.Id)
            .ToArray();

        // An empty AncestorIds filter matches the episodes of every series. Without this return, a series that is not in the library would find the same episode of every series.
        if (seriesIds.Length == 0)
        {
            return [];
        }

        return libraryManager.GetItemList(new InternalItemsQuery(user)
        {
            IncludeItemTypes = [BaseItemKind.Episode],
            AncestorIds = seriesIds,
            ParentIndexNumber = id.Season,
            IndexNumber = id.Episode,
        });
    }
}
