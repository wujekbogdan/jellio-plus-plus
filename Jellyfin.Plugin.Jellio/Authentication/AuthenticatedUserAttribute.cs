using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.Jellio.Authentication;

/// <summary>
/// Binds the Jellyfin user of the request.
/// Use it only on actions that have <see cref="ConfigAuthorizeAttribute"/>, because that attribute authenticates the user.
/// </summary>
public sealed class AuthenticatedUserAttribute() : ModelBinderAttribute(typeof(AuthenticatedUserModelBinder));
