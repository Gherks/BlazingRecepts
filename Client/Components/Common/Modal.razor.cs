using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.Common;

public partial class Modal
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "Modal";

    private HxModal? _modal;

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment? Body { get; set; }

    [Parameter]
    public RenderFragment? Buttons { get; set; }

    public void Open()
    {
        if (_modal == null)
        {
            const string errorMessage = "Cannot open modal because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _modal.ShowAsync();
        StateHasChanged();
    }

    public void Close()
    {
        if (_modal == null)
        {
            const string errorMessage = "Cannot close modal because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _modal.HideAsync();
        StateHasChanged();
    }
}
