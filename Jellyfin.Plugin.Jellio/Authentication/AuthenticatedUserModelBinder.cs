using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Jellyfin.Plugin.Jellio.Authentication;

internal sealed class AuthenticatedUserModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var feature = bindingContext.HttpContext.Features.Get<AuthenticatedUserFeature>();
        bindingContext.Result = ModelBindingResult.Success(feature!.User);
        return Task.CompletedTask;
    }
}
