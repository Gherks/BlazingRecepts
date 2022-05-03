using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.Common;

public partial class RemovalConfirmationModal<Type> : ComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RemovalConfirmationModal";

    private Type? _entity = default;
    private string _title = string.Empty;
    private string _name = string.Empty;
    private HxModal? _modal;

    [Parameter]
    public Func<Type, Task>? OnConfirm { get; set; }

    public async Task Open(Type entity, string title, string name)
    {
        if (_modal == null)
        {
            const string errorMessage = "Modal cannot be opened because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _title = title;
        _name = name;
        _entity = entity;

        await _modal.ShowAsync();
    }

    private async Task HandleConfirm()
    {
        if (_modal == null)
        {
            const string errorMessage = "Modal cannot be closed because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (OnConfirm == null)
        {
            const string errorMessage = "Cannot call the on confirm routine because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_entity == null)
        {
            const string errorMessage = "Entity cannot be removed because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await OnConfirm.Invoke(_entity);
        await _modal.HideAsync();
    }

    private async Task HandleCancel()
    {
        if (_modal == null)
        {
            const string errorMessage = "Modal cannot be closed because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await _modal.HideAsync();
    }
}
