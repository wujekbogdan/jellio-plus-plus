using System.Collections.Generic;

namespace Jellyfin.Plugin.Jellio.Subtitles;

internal abstract record SubtitleOutcome
{
    private SubtitleOutcome()
    {
    }

    internal sealed record TitleNotInLibrary : SubtitleOutcome;

    /// <summary>
    /// No version of the title is the playing file, or the request has no playing file.
    /// </summary>
    internal sealed record NoMatchingVersion : SubtitleOutcome;

    /// <summary>
    /// No track of the playing version can be offered. <c>Skipped</c> is the number of subtitle tracks that the version has, and is 0 when it has none.
    /// </summary>
    internal sealed record NoUsableTracks(int Skipped) : SubtitleOutcome;

    /// <summary>
    /// The tracks are in display order.
    /// </summary>
    internal sealed record Offered(IReadOnlyList<SubtitleTrack> Tracks) : SubtitleOutcome;
}
