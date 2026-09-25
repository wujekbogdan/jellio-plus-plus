namespace Jellyfin.Plugin.Jellio.Subtitles;

/// <summary>
/// The file that plays. <c>Filename</c> is the file name without its directory.
/// </summary>
internal sealed record PlayingFile(string Filename, long Size);
