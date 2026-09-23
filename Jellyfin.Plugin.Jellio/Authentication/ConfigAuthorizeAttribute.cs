using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.Jellio.Authentication;

public sealed class ConfigAuthorizeAttribute() : TypeFilterAttribute(typeof(ConfigAuthFilter));
