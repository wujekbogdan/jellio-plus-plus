using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Plugin.Jellio.Authentication;

internal sealed record AuthenticatedUserFeature(User User);
