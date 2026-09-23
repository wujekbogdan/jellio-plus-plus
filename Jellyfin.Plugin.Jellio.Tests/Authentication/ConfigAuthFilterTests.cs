using System.Text;
using System.Text.Json;
using Jellyfin.Data.Queries;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Plugin.Jellio.Authentication;
using MediaBrowser.Controller.Devices;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;

namespace Jellyfin.Plugin.Jellio.Tests.Authentication;

public class ConfigAuthFilterTests
{
    private readonly IUserManager _userManager = Substitute.For<IUserManager>();
    private readonly IDeviceManager _deviceManager = Substitute.For<IDeviceManager>();

    [Fact]
    public async Task ValidKey_HandsTheUserToTheAction()
    {
        var user = new User("alice", "provider", "reset-provider") { Id = Guid.NewGuid() };
        var device = new Device(user.Id, "Jellio++", "1.0", "Stremio", "device-1");
        _deviceManager.GetDevices(Arg.Any<DeviceQuery>()).Returns(new QueryResult<Device>([device]));
        _userManager.GetUserById(user.Id).Returns(user);

        var actionContext = CreateActionContext(EncodeConfig(device.AccessToken));
        var authorizationContext = new AuthorizationFilterContext(actionContext, []);

        await new ConfigAuthFilter(_userManager, _deviceManager).OnAuthorizationAsync(authorizationContext);

        Assert.Null(authorizationContext.Result);
        Assert.Same(user, await BindAuthenticatedUser(actionContext));
    }

    [Fact]
    public async Task UnknownKey_IsRejectedAsUnauthorized()
    {
        _deviceManager.GetDevices(Arg.Any<DeviceQuery>()).Returns(new QueryResult<Device>([]));

        var authorizationContext = new AuthorizationFilterContext(CreateActionContext(EncodeConfig("unknown-token")), []);

        await new ConfigAuthFilter(_userManager, _deviceManager).OnAuthorizationAsync(authorizationContext);

        Assert.IsType<UnauthorizedResult>(authorizationContext.Result);
    }

    [Fact]
    public async Task KeyOfADeletedUser_IsRejectedAsUnauthorized()
    {
        var device = new Device(Guid.NewGuid(), "Jellio++", "1.0", "Stremio", "device-1");
        _deviceManager.GetDevices(Arg.Any<DeviceQuery>()).Returns(new QueryResult<Device>([device]));

        var authorizationContext = new AuthorizationFilterContext(CreateActionContext(EncodeConfig(device.AccessToken)), []);

        await new ConfigAuthFilter(_userManager, _deviceManager).OnAuthorizationAsync(authorizationContext);

        Assert.IsType<UnauthorizedResult>(authorizationContext.Result);
    }

    [Theory]
    [InlineData("not base64 !")]
    [InlineData("bm90IGpzb24")] // "not json"
    [InlineData("e30")] // "{}", no required fields
    [InlineData("bnVsbA")] // "null"
    public async Task BrokenAddonLink_IsRejectedAsBadRequest(string encodedConfig)
    {
        var authorizationContext = new AuthorizationFilterContext(CreateActionContext(encodedConfig), []);

        await new ConfigAuthFilter(_userManager, _deviceManager).OnAuthorizationAsync(authorizationContext);

        Assert.IsType<BadRequestObjectResult>(authorizationContext.Result);
    }

    private static string EncodeConfig(string authToken)
    {
        var json = JsonSerializer.Serialize(new { ServerName = "Home", AuthToken = authToken, LibrariesGuids = Array.Empty<Guid>() });
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(json));
    }

    private static ActionContext CreateActionContext(string encodedConfig)
    {
        var routeData = new RouteData();
        routeData.Values["config"] = encodedConfig;
        return new ActionContext(new DefaultHttpContext(), routeData, new ActionDescriptor());
    }

    private static async Task<object?> BindAuthenticatedUser(ActionContext actionContext)
    {
        var metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(User));
        var bindingContext = DefaultModelBindingContext.CreateBindingContext(
            actionContext,
            new RouteValueProvider(BindingSource.Path, actionContext.RouteData.Values),
            metadata,
            bindingInfo: null,
            modelName: "user");

        await new AuthenticatedUserModelBinder().BindModelAsync(bindingContext);

        return bindingContext.Result.Model;
    }
}
