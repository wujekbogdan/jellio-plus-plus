namespace Jellyfin.Plugin.Jellio.Library;

/// <summary>
/// An episode, given by the IMDb id of its series (with the <c>tt</c> prefix) and its season and episode numbers.
/// </summary>
public readonly record struct ImdbEpisodeId(string SeriesImdbId, int Season, int Episode);
