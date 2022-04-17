namespace BlazingRecept.Client.Shared;

public partial class NavMenu
{
    private bool _collapseNavMenu = true;

    private string? NavMenuCssClass => _collapseNavMenu ? "collapse" : null;

    private void HandleNavMenuToggle()
    {
        _collapseNavMenu = !_collapseNavMenu;
    }
}
