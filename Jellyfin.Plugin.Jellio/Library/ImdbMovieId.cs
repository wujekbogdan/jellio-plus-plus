namespace Jellyfin.Plugin.Jellio.Library;

/// <summary>
/// The IMDb id of a movie, with the <c>tt</c> prefix, for example <c>tt0133093</c>.
/// </summary>
public readonly record struct ImdbMovieId(string Value);
