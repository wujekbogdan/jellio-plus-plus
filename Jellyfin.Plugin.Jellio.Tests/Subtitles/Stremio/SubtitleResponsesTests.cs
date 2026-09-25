using System.Text.Json;
using System.Text.Json.Nodes;
using Jellyfin.Plugin.Jellio.Subtitles;
using Jellyfin.Plugin.Jellio.Subtitles.Stremio;

namespace Jellyfin.Plugin.Jellio.Tests.Subtitles.Stremio;

public class SubtitleResponsesTests
{
    private const string BaseUrl = "https://jellyfin.example.com";
    private const string AuthToken = "token/with+special=chars";

    private static readonly Guid ItemId = Guid.Parse("7e9a1b2c-3d4e-4f50-8a6b-7c8d9e0f1a2b");

    [Fact]
    public void OfferedTrack_BecomesOneSubtitleWithAnAuthenticatedDownloadUrl()
    {
        var track = new SubtitleTrack(ItemId, "a1b2c3", StreamIndex: 4, Language: "eng", SubtitleFormat.Srt, Label: "SDH", FontAttachmentIndexes: []);

        var response = SubtitleResponses.From(new SubtitleOutcome.Offered([track]), baseUrl: BaseUrl, authToken: AuthToken);

        Assert.Equivalent(
            new SubtitleDto(
                Id: "jelliopp:a1b2c3:4",
                Url: $"{BaseUrl}/Videos/{ItemId}/a1b2c3/Subtitles/4/0/Stream.srt?ApiKey=token%2Fwith%2Bspecial%3Dchars",
                Lang: "eng",
                Label: "SDH",
                Fonts: null),
            Assert.Single(response.Subtitles),
            strict: true);
    }

    [Fact]
    public void StyledTrack_HasAnAssUrlAndTheFontUrlsOfItsVersion()
    {
        var styled = new SubtitleTrack(ItemId, "a1b2c3", StreamIndex: 5, Language: "eng", SubtitleFormat.Ass, Label: null, FontAttachmentIndexes: [9, 11]);
        var plain = new SubtitleTrack(ItemId, "a1b2c3", StreamIndex: 6, Language: "pol", SubtitleFormat.Srt, Label: null, FontAttachmentIndexes: []);

        var subtitles = SubtitleResponses.From(new SubtitleOutcome.Offered([styled, plain]), baseUrl: BaseUrl, authToken: AuthToken).Subtitles;

        Assert.Matches(@"/Stream\.ass\?", subtitles[0].Url);
        Assert.Equal(
            [
                $"{BaseUrl}/Videos/{ItemId}/a1b2c3/Attachments/9?ApiKey=token%2Fwith%2Bspecial%3Dchars",
                $"{BaseUrl}/Videos/{ItemId}/a1b2c3/Attachments/11?ApiKey=token%2Fwith%2Bspecial%3Dchars",
            ],
            subtitles[0].Fonts);
        Assert.Null(subtitles[1].Fonts);
    }

    [Fact]
    public void TrackWithoutLabelOrFonts_IsSerializedWithoutThoseKeys()
    {
        var track = new SubtitleTrack(ItemId, "a1b2c3", StreamIndex: 4, Language: "eng", SubtitleFormat.Srt, Label: null, FontAttachmentIndexes: []);

        var json = JsonSerializer.Serialize(SubtitleResponses.From(new SubtitleOutcome.Offered([track]), baseUrl: BaseUrl, authToken: AuthToken));

        Assert.Equal(["id", "url", "lang"], JsonNode.Parse(json)!["subtitles"]![0]!.AsObject().Select(property => property.Key));
    }

    [Theory]
    [InlineData(nameof(SubtitleOutcome.TitleNotInLibrary))]
    [InlineData(nameof(SubtitleOutcome.NoMatchingVersion))]
    [InlineData(nameof(SubtitleOutcome.NoUsableTracks))]
    public void OutcomeWithoutTracks_IsSerializedAsAnEmptySubtitleList(string outcomeName)
    {
        SubtitleOutcome outcome = outcomeName switch
        {
            nameof(SubtitleOutcome.TitleNotInLibrary) => new SubtitleOutcome.TitleNotInLibrary(),
            nameof(SubtitleOutcome.NoMatchingVersion) => new SubtitleOutcome.NoMatchingVersion(VersionCount: 1),
            _ => new SubtitleOutcome.NoUsableTracks(Skipped: 3),
        };

        var json = JsonSerializer.Serialize(SubtitleResponses.From(outcome, baseUrl: BaseUrl, authToken: AuthToken));

        Assert.Equal("""{"subtitles":[]}""", json);
    }
}
