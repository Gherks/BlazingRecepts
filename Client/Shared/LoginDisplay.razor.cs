using BlazingRecept.Contract;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazingRecept.Client.Shared;

public partial class LoginDisplay
{
    [Inject]
    protected internal NavigationManager? NavigationManager { get; private set; }

    private void HandleBeginLogin()
    {
        Contracts.LogAndThrowWhenNull(NavigationManager, "Cannot handle begin login because navigation manager has not been set.");

        NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
    }

    private void HandleBeginLogout(MouseEventArgs args)
    {
        Contracts.LogAndThrowWhenNull(NavigationManager, "Cannot handle begin logout because navigation manager has not been set.");

        NavigationManager.NavigateToLogout("authentication/logout");
    }
}
