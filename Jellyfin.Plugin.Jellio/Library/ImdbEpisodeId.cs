namespace Jellyfin.Plugin.Jellio.Library;

/// <summary>
/// An episode, given by the IMDb id of its series and its season and episode numbers. The series IMDb id has the <c>tt</c> prefix, for example <c>tt0903747</c>.
/// </summary>
public readonly record struct ImdbEpisodeId(string SeriesImdbId, int Season, int Episode);
