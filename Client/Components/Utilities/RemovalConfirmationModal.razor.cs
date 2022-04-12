using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.Utilities;

public partial class RemovalConfirmationModal<Type> : ComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RemovalConfirmationModal";

    private Type? _entity = default(Type);
    private string _title = string.Empty;
    private string _name = string.Empty;
    private Modal? _modal;

    [Parameter]
    public Func<Type, Task>? OnConfirm { get; set; }

    public void Open(Type entity, string title, string name)
    {
        if (_modal == null)
        {
            string errorMessage = "Modal cannot be opened because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _title = title;
        _name = name;
        _entity = entity;

        _modal.Open();
    }

    private void HandleConfirm()
    {
        if (_modal == null)
        {
            string errorMessage = "Modal cannot be closed because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (OnConfirm == null)
        {
            string errorMessage = "Cannot call the on confirm routine because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_entity == null)
        {
            string errorMessage = "Entity cannot be removed because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        OnConfirm.Invoke(_entity);
        _modal.Close();
    }

    private void HandleCancel()
    {
        if (_modal == null)
        {
            string errorMessage = "Modal cannot be closed because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _modal.Close();
    }
}
