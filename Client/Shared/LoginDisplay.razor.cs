using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Serilog;

namespace BlazingRecept.Client.Shared;

public partial class LoginDisplay
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "LoginDisplay";

    [Inject]
    protected internal NavigationManager? NavigationManager { get; private set; }

    [Inject]
    protected internal SignOutSessionStateManager? SignOutSessionStateManager { get; private set; }

    private void HandleBeginLogin()
    {
        if (NavigationManager == null)
        {
            const string errorMessage = "Cannot handle begin login because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
    }

    private async Task HandleBeginLogout(MouseEventArgs args)
    {
        if (NavigationManager == null)
        {
            const string errorMessage = "Cannot handle begin logout because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (SignOutSessionStateManager == null)
        {
            const string errorMessage = "Cannot handle begin logout because sign out session state manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await SignOutSessionStateManager.SetSignOutState();
        NavigationManager.NavigateTo("authentication/logout");
    }
}
