using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.Jellio.Authentication;

/// <summary>
/// Binds an action parameter to the Jellyfin user that <see cref="ConfigAuthorizeAttribute"/> authenticated.
/// The action must have <see cref="ConfigAuthorizeAttribute"/>, otherwise the binding throws.
/// </summary>
public sealed class AuthenticatedUserAttribute() : ModelBinderAttribute(typeof(AuthenticatedUserModelBinder));
