using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Utilities;

public partial class Modal
{
    private string _modalDisplay = "none";
    private string _modalClass = "";

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string Options { get; set; } = string.Empty;

    [Parameter]
    public bool CloseButtonDisabled { get; set; } = false;

    [Parameter]
    public RenderFragment? Body { get; set; }

    [Parameter]
    public RenderFragment? Buttons { get; set; }

    public Func<Task>? OnCloseAsync { get; set; }

    public void Open()
    {
        _modalDisplay = "block;";
        _modalClass = "Show";
        StateHasChanged();
    }

    public async Task CloseAsync()
    {
        _modalDisplay = "none";
        _modalClass = "";
        StateHasChanged();

        if (OnCloseAsync != null)
        {
            await OnCloseAsync.Invoke();
        }
    }
}
