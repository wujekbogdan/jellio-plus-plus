using Microsoft.AspNetCore.Http;

namespace Jellyfin.Plugin.Jellio.Controllers;

internal static class JellyfinBaseUrl
{
    public static string Of(HttpRequest request, string? publicBaseUrl) =>
        string.IsNullOrWhiteSpace(publicBaseUrl)
            ? $"{request.Scheme}://{request.Host}{request.PathBase}"
            : publicBaseUrl.TrimEnd('/');
}
