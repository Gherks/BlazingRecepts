using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Serilog;

namespace BlazingRecept.Client.Utilities;

public sealed class CustomValidation : ComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "CustomValidation";

    private ValidationMessageStore? _messageStore;

    [CascadingParameter]
    internal EditContext? CurrentEditContext { get; private set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            const string errorMessage = $"{nameof(CustomValidation)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(CustomValidation)} " +
                $"inside an {nameof(EditForm)}.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
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

    public bool ContainsError(string errorName)
    {
        if (_messageStore != null && CurrentEditContext != null)
        {
            IReadOnlyList<string> fields = _messageStore[CurrentEditContext.Field(errorName)].ToList();

            return fields.Count != 0;
        }

        return false;
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
