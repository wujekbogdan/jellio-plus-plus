using Jellyfin.Data.Enums;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Plugin.Jellio.Library;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using NSubstitute;

namespace Jellyfin.Plugin.Jellio.Tests.Library;

public class ItemResolverTests
{
    private static readonly ImdbEpisodeId BreakingBadS02E05 = new("tt0903747", Season: 2, Episode: 5);

    private readonly ILibraryManager _libraryManager = Substitute.For<ILibraryManager>();
    private readonly User _user = new("alice", "provider", "reset-provider") { Id = Guid.NewGuid() };

    [Fact]
    public void LibraryItemId_FindsThatItemOrNothing()
    {
        var movie = new Movie { Id = Guid.NewGuid() };
        _libraryManager.GetItemById<BaseItem>(movie.Id, _user.Id).Returns(movie);
        var resolver = new ItemResolver(_libraryManager);

        Assert.Equal([movie], resolver.Resolve(_user, new LibraryItemId(movie.Id)));
        Assert.Empty(resolver.Resolve(_user, new LibraryItemId(Guid.NewGuid())));
    }

    [Fact]
    public void ImdbMovieId_FindsTheUsersMoviesWithThatId()
    {
        var movies = new[] { new Movie { Id = Guid.NewGuid() }, new Movie { Id = Guid.NewGuid() } };
        _libraryManager
            .GetItemList(Arg.Is<InternalItemsQuery>(query =>
                query.User == _user
                && query.IncludeItemTypes.SequenceEqual(new[] { BaseItemKind.Movie })
                && query.HasAnyProviderId!["Imdb"] == "tt0133093"))
            .Returns(movies);

        Assert.Equal(movies, new ItemResolver(_libraryManager).Resolve(_user, new ImdbMovieId("tt0133093")));
    }

    [Fact]
    public void ImdbEpisodeId_FindsTheEpisodeInsideTheSeriesWithThatId()
    {
        var series = new Series { Id = Guid.NewGuid() };
        var episode = new Episode { Id = Guid.NewGuid() };
        _libraryManager
            .GetItemList(Arg.Is<InternalItemsQuery>(query =>
                query.User == _user
                && query.IncludeItemTypes.SequenceEqual(new[] { BaseItemKind.Series })
                && query.HasAnyProviderId!["Imdb"] == BreakingBadS02E05.SeriesImdbId))
            .Returns([series]);
        _libraryManager
            .GetItemList(Arg.Is<InternalItemsQuery>(query =>
                query.User == _user
                && query.IncludeItemTypes.SequenceEqual(new[] { BaseItemKind.Episode })
                && query.AncestorIds.SequenceEqual(new[] { series.Id })
                && query.ParentIndexNumber == BreakingBadS02E05.Season
                && query.IndexNumber == BreakingBadS02E05.Episode))
            .Returns([episode]);

        Assert.Equal([episode], new ItemResolver(_libraryManager).Resolve(_user, BreakingBadS02E05));
    }

    [Fact]
    public void ImdbEpisodeId_FindsNothingWhenNoSeriesHasThatId()
    {
        // An empty AncestorIds filter matches episodes of every series.
        _libraryManager
            .GetItemList(Arg.Is<InternalItemsQuery>(query => query.IncludeItemTypes.SequenceEqual(new[] { BaseItemKind.Episode })))
            .Returns([new Episode { Id = Guid.NewGuid() }]);

        Assert.Empty(new ItemResolver(_libraryManager).Resolve(_user, BreakingBadS02E05));
    }
}
