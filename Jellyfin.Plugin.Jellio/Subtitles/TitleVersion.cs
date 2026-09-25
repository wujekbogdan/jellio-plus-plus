using System;
using MediaBrowser.Model.Dto;

namespace Jellyfin.Plugin.Jellio.Subtitles;

/// <summary>
/// One version of a title: one media source of one library item. A title can have several versions, as several library items, several media sources of one item, or both.
/// </summary>
internal sealed record TitleVersion(Guid ItemId, MediaSourceInfo Source);
