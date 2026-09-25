using System;
using System.Net.Mime;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Plugin.Jellio.Authentication;
using Jellyfin.Plugin.Jellio.Helpers;
using Jellyfin.Plugin.Jellio.Library;
using Jellyfin.Plugin.Jellio.Models;
using Jellyfin.Plugin.Jellio.Subtitles;
using Jellyfin.Plugin.Jellio.Subtitles.Stremio;
using MediaBrowser.Controller.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.Jellio.Controllers;

[ApiController]
[ConfigAuthorize]
[Route("jelliopp/{config}/subtitles")]
[Produces(MediaTypeNames.Application.Json)]
public class SubtitleController(ItemResolver itemResolver, IDtoService dtoService) : ControllerBase
{
    private readonly SubtitleLookup _lookup = new(itemResolver, dtoService);

    [HttpGet("{stremioType}/jelliopp:{mediaId:guid}.json")]
    [HttpGet("{stremioType}/jelliopp:{mediaId:guid}/{extra}.json")]
    public IActionResult GetLibraryItemSubtitles([ConfigFromBase64Json] ConfigModel config, [AuthenticatedUser] User user, Guid mediaId) =>
        Respond(config, $"jelliopp:{mediaId}", playingFile => _lookup.For(user, new LibraryItemId(mediaId), playingFile));

    [HttpGet("movie/tt{imdbId}.json")]
    [HttpGet("movie/tt{imdbId}/{extra}.json")]
    public IActionResult GetMovieSubtitles([ConfigFromBase64Json] ConfigModel config, [AuthenticatedUser] User user, string imdbId) =>
        Respond(config, $"tt{imdbId}", playingFile => _lookup.For(user, new ImdbMovieId($"tt{imdbId}"), playingFile));

    [HttpGet("series/tt{imdbId}:{seasonNum:int}:{episodeNum:int}.json")]
    [HttpGet("series/tt{imdbId}:{seasonNum:int}:{episodeNum:int}/{extra}.json")]
    public IActionResult GetEpisodeSubtitles([ConfigFromBase64Json] ConfigModel config, [AuthenticatedUser] User user, string imdbId, int seasonNum, int episodeNum) =>
        Respond(config, $"tt{imdbId}:{seasonNum}:{episodeNum}", playingFile => _lookup.For(user, new ImdbEpisodeId($"tt{imdbId}", seasonNum, episodeNum), playingFile));

    private OkObjectResult Respond(ConfigModel config, string requestedId, Func<PlayingFile?, SubtitleOutcome> lookup)
    {
        var playingFile = PlayingFileExtra.FromRawTarget(HttpContext.Features.GetRequiredFeature<IHttpRequestFeature>().RawTarget);
        var outcome = lookup(playingFile);
        LogBuffer.AddLog(SubtitleLogLine.For(requestedId, playingFile, outcome), LogLevel.Info);
        return Ok(SubtitleResponses.From(outcome, baseUrl: JellyfinBaseUrl.Of(Request, config.PublicBaseUrl), authToken: config.AuthToken));
    }
}
