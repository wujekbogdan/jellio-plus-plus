using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Jellio.Subtitles;

internal static class FontAttachments
{
    // The font media types and file endings of the Matroska specification (matroska.org, "Attachments"). The legacy types occur in files that are older than RFC 8081.
    private static readonly string[] FontMimeTypes =
    [
        "font/sfnt",
        "font/ttf",
        "font/otf",
        "font/collection",
        "font/woff",
        "font/woff2",
        "application/x-truetype-font",
        "application/x-font-ttf",
        "application/vnd.ms-opentype",
        "application/font-sfnt",
        "application/font-woff",
    ];

    private static readonly string[] FontFileEndings = [".ttf", ".otf", ".ttc"];

    public static IReadOnlyList<int> In(MediaSourceInfo source) =>
        source.MediaAttachments
            .Where(IsFont)
            .Select(attachment => attachment.Index)
            .ToList();

    private static bool IsFont(MediaAttachment attachment) =>
        FontMimeTypes.Contains(attachment.MimeType, StringComparer.OrdinalIgnoreCase)
        || FontFileEndings.Contains(Path.GetExtension(attachment.FileName), StringComparer.OrdinalIgnoreCase);
}
