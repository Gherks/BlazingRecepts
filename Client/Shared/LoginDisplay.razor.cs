using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazingRecept.Client.Shared;

public partial class LoginDisplay
{
    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    [Inject]
    public SignOutSessionStateManager? SignOutSessionStateManager { get; set; }

    private void HandleBeginLogin()
    {
        if (NavigationManager == null) throw new InvalidOperationException();

        NavigationManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
    }

    private async Task HandleBeginLogout(MouseEventArgs args)
    {
        if (NavigationManager == null) throw new InvalidOperationException();
        if (SignOutSessionStateManager == null) throw new InvalidOperationException();

        await SignOutSessionStateManager.SetSignOutState();
        NavigationManager.NavigateTo("authentication/logout");
    }
}
