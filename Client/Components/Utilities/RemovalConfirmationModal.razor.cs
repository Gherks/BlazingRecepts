using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Utilities;

public partial class RemovalConfirmationModal<Type> : ComponentBase
{
    private Modal? _modal;

    private Type? _type = default(Type);

    private string _title = string.Empty;

    private string _name = string.Empty;

    [Parameter]
    public Func<Type, Task>? OnConfirm { get; set; }

    public void Open(Type type, string title, string name)
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be opened because it has not been set.");

        _title = title;
        _name = name;
        _type = type;

        _modal.Open();
    }

    private void HandleConfirm()
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be closed because it has not been set.");
        if (OnConfirm == null) throw new InvalidOperationException("Cannot call the on confirm routine because it has not been set.");
        if (_type == null) throw new InvalidOperationException("Type cannot be removed because it has not been set.");

        OnConfirm.Invoke(_type);
        _modal.Close();
    }

    private void HandleCancel()
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be closed because it has not been set.");

        _modal.Close();
    }
}
