using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jellyfin.Plugin.Jellio.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace Jellyfin.Plugin.Jellio.Helpers;

/// <summary>
/// Decodes the Base64Url-encoded JSON configuration of an addon link.
/// Returns <c>null</c> when the value is missing or is not a valid configuration.
/// </summary>
internal static class ConfigDecoder
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false) },
    };

    public static ConfigModel? Decode(string? encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<ConfigModel>(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encoded)), Options);
        }
        catch (FormatException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
