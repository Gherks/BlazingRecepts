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
    public NavigationManager? NavigationManager { get; set; }

    [Inject]
    public SignOutSessionStateManager? SignOutSessionStateManager { get; set; }

    private void HandleBeginLogin()
    {
        if (NavigationManager == null)
        {
            string errorMessage = "Cannot handle begin login because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
    }

    private async Task HandleBeginLogout(MouseEventArgs args)
    {
        if (NavigationManager == null)
        {
            string errorMessage = "Cannot handle begin logout because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (SignOutSessionStateManager == null)
        {
            string errorMessage = "Cannot handle begin logout because sign out session state manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await SignOutSessionStateManager.SetSignOutState();
        NavigationManager.NavigateTo("authentication/logout");
    }
}
