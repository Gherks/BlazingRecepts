using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazingRecept.Client.Pages.Base;

public class PageBase : ComponentBase
{
    private ClaimsPrincipal? _claimsPrincipal;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    public bool IsAuthenticated
    {
        get
        {
            if (_claimsPrincipal == null) throw new InvalidOperationException();
            if (_claimsPrincipal.Identity == null) throw new InvalidOperationException();

            return _claimsPrincipal.Identity.IsAuthenticated;
        }

        private set { }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (AuthenticationStateTask == null) throw new InvalidOperationException();

        AuthenticationState authenticationState = await AuthenticationStateTask;
        _claimsPrincipal = authenticationState.User;
    }
}
