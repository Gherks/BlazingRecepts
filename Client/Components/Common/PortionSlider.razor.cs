using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazingRecept.Client.Components.Common;

public partial class PortionSlider : ComponentBase, IAsyncDisposable
{
    private const int MIN_VALUE = 1;
    private const int MAX_VALUE = 16;
    private DotNetObjectReference<PortionSlider>? _dotNetReference;
    private IJSObjectReference? _jsModule;
    private bool _isInitialized = false;

    [Parameter]
    public int Value { get; set; } = 1;

    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    [Inject]
    private IJSRuntime? JSRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && JSRuntime != null && !_isInitialized)
        {
            _dotNetReference = DotNetObjectReference.Create(this);
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/portionSlider.js");
            
            await _jsModule.InvokeVoidAsync("initializePortionSlider", _dotNetReference);
            _isInitialized = true;
        }
    }

    [JSInvokable]
    public async Task UpdateValue(int newValue)
    {
        if (newValue >= MIN_VALUE && newValue <= MAX_VALUE && newValue != Value)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
            StateHasChanged();
        }
    }

    private string GetKnobPosition()
    {
        double percentage = (Value - MIN_VALUE) / (double)(MAX_VALUE - MIN_VALUE) * 100.0;
        return $"{percentage}%";
    }

    private async Task HandleMouseDown(MouseEventArgs e)
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("startDrag", "mouse");
        }
    }

    private async Task HandleTouchStart(TouchEventArgs e)
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("startDrag", "touch");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("cleanup");
            await _jsModule.DisposeAsync();
        }
        _dotNetReference?.Dispose();
    }
}
