using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

internal static class TrackRanks
{
    public static TrackRank Of(MediaStream track) => track switch
    {
        { IsDefault: true } => TrackRank.Default,
        { IsHearingImpaired: true } => TrackRank.HearingImpaired,
        { IsForced: true } => TrackRank.Forced,
        { Title.Length: > 0 } => TrackRank.Titled,
        _ => TrackRank.Normal,
    };
}
