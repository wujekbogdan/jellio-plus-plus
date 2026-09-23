using System.Threading.Tasks;
using Jellyfin.Plugin.Jellio.Helpers;
using MediaBrowser.Controller.Devices;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Jellyfin.Plugin.Jellio.Authentication;

/// <summary>
/// Authenticates an addon request by the access token in its addon link.
/// It must stay an authorization filter: model binding runs after authorization filters and before action filters, and <see cref="AuthenticatedUserAttribute"/> parameters need the user that this filter sets.
/// </summary>
public class ConfigAuthFilter(IUserManager userManager, IDeviceManager deviceManager)
    : IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var config = ConfigDecoder.Decode(context.RouteData.Values["config"] as string);
        if (config is null)
        {
            context.Result = new BadRequestObjectResult("Invalid or missing configuration");
            return Task.CompletedTask;
        }

        var userId = RequestHelpers.GetUserIdByAuthToken(config.AuthToken, deviceManager);
        var user = userId is null ? null : userManager.GetUserById(userId.Value);
        if (user is null)
        {
            context.Result = new UnauthorizedResult();
            return Task.CompletedTask;
        }

        context.HttpContext.Features.Set(new AuthenticatedUserFeature(user));
        return Task.CompletedTask;
    }
}
