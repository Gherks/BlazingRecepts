using BlazingRecept.Shared;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Common;

public partial class RemovalConfirmationModal<Type> : ComponentBase
{
    private Type? _entity = default;
    private string _title = string.Empty;
    private string _name = string.Empty;
    private HxModal? _modal;

    [Parameter]
    public Func<Type, Task>? OnConfirm { get; set; }

    public async Task Open(Type entity, string title, string name)
    {
        Contracts.LogAndThrowWhenNull(_modal, "Modal cannot be opened because it has not been set.");

        _title = title;
        _name = name;
        _entity = entity;
        StateHasChanged();

        await _modal.ShowAsync();
    }

    private async Task HandleConfirm()
    {
        Contracts.LogAndThrowWhenNull(_modal, "Modal cannot be closed because it has not been set.");
        Contracts.LogAndThrowWhenNull(OnConfirm, "Cannot call the on confirm routine because it has not been set.");
        Contracts.LogAndThrowWhenNull(_entity, "Entity cannot be removed because it has not been set.");

        await OnConfirm.Invoke(_entity);
        await _modal.HideAsync();
    }

    private async Task HandleCancel()
    {
        Contracts.LogAndThrowWhenNull(_modal, "Modal cannot be closed because it has not been set.");

        await _modal.HideAsync();
    }
}
