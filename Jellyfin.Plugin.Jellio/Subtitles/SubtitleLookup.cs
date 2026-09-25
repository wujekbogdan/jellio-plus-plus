using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Plugin.Jellio.Library;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

internal sealed class SubtitleLookup(ItemResolver itemResolver, IDtoService dtoService)
{
    public SubtitleOutcome For(User user, LibraryItemId id, PlayingFile? playingFile) => Select(user, itemResolver.Resolve(user, id), playingFile);

    public SubtitleOutcome For(User user, ImdbMovieId id, PlayingFile? playingFile) => Select(user, itemResolver.Resolve(user, id), playingFile);

    public SubtitleOutcome For(User user, ImdbEpisodeId id, PlayingFile? playingFile) => Select(user, itemResolver.Resolve(user, id), playingFile);

    private SubtitleOutcome Select(User user, IReadOnlyList<BaseItem> items, PlayingFile? playingFile)
    {
        var versions = dtoService
            .GetBaseItemDtos(items, new DtoOptions(true), user)
            .SelectMany(dto => (dto.MediaSources ?? []).Select(source => new TitleVersion(dto.Id, source)))
            .ToList();

        return SubtitleSelection.For(new SubtitleRequest(versions, playingFile));
    }
}
