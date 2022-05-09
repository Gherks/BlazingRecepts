using BlazingRecept.Contract;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazingRecept.Client.Shared;

public partial class LoginDisplay
{
    [Inject]
    protected internal NavigationManager? NavigationManager { get; private set; }

    [Inject]
    protected internal SignOutSessionStateManager? SignOutSessionStateManager { get; private set; }

    private void HandleBeginLogin()
    {
        Contracts.LogAndThrowWhenNull(NavigationManager, "Cannot handle begin login because navigation manager has not been set.");

        NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
    }

    private async Task HandleBeginLogout(MouseEventArgs args)
    {
        Contracts.LogAndThrowWhenNull(NavigationManager, "Cannot handle begin logout because navigation manager has not been set.");
        Contracts.LogAndThrowWhenNull(SignOutSessionStateManager, "Cannot handle begin logout because sign out session state manager has not been set.");

        await SignOutSessionStateManager.SetSignOutState();
        NavigationManager.NavigateTo("authentication/logout");
    }
}
