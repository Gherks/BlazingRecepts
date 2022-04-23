using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Utilities;

public partial class CollapsibleComponent : ComponentBase
{
    private bool _showChildContent = false;
    private bool _shouldTriggerEvent = false;
    private string _buttonText => _showChildContent ? ExposedButtonText : CollapsedButtonText;
    private string _class => _showChildContent ? "mb-2" : string.Empty;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string CollapsedButtonText { get; set; } = string.Empty;

    [Parameter]
    public string ExposedButtonText { get; set; } = string.Empty;

    [Parameter]
    public bool ShowChildContentOnFirstRender { get; set; } = false;

    [Parameter]
    public Func<Task>? OnShow { get; set; } = null;

    [Parameter]
    public Func<Task>? OnHide { get; set; } = null;

    public bool IsShowingContent() => _showChildContent;

    public void HandleToggle()
    {
        _showChildContent = !_showChildContent;
        _shouldTriggerEvent = true;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender && ShowChildContentOnFirstRender)
        {
            _showChildContent = true;
        }
        else if (_shouldTriggerEvent)
        {
            _shouldTriggerEvent = false;

            if (_showChildContent)
            {
                if (OnShow != null)
                {
                    OnShow.Invoke();
                }
            }
            else
            {
                if (OnHide != null)
                {
                    OnHide.Invoke();
                }
            }
        }
    }
}
