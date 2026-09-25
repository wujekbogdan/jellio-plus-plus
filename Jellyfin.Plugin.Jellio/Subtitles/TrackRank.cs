namespace Jellyfin.Plugin.Jellio.Subtitles;

/// <summary>
/// The rank of a subtitle track among the tracks of its language. The members are in display order.
/// </summary>
internal enum TrackRank
{
    Default,
    Normal,
    HearingImpaired,
    Forced,
    Titled,
}
