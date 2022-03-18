using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazingRecept.Client.Utilities;

public sealed class CustomValidation : ComponentBase
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException(
                $"{nameof(CustomValidation)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(CustomValidation)} " +
                $"inside an {nameof(EditForm)}.");
        }

        _messageStore = new(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += (sender, fieldChangedEventArgs) => _messageStore.Clear();
        CurrentEditContext.OnFieldChanged += (sender, fieldChangedEventArgs) => _messageStore.Clear(fieldChangedEventArgs.FieldIdentifier);
    }

    public void DisplayErrors(Dictionary<string, List<string>> errors)
    {
        if (_messageStore != null && CurrentEditContext != null)
        {
            foreach (var error in errors)
            {
                _messageStore.Add(CurrentEditContext.Field(error.Key), error.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void RemoveError(string errorName)
    {
        if (_messageStore != null && CurrentEditContext != null)
        {
            _messageStore.Clear(CurrentEditContext.Field(errorName));
        }
    }

    public void ClearErrors()
    {
        if (_messageStore != null && CurrentEditContext != null)
        {
            _messageStore.Clear();
            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
