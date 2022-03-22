using BlazingRecept.Client.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazingRecept.Client.Pages;

public partial class CreateRecipe : ComponentBase
{
    private readonly string _editFormId = "CreateRecipeEditForm";
    private readonly Form _form = new();

    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    public CreateRecipe()
    {
        _editContext = new(_form);
    }

    private void ValidFormSubmitted()
    {
        if (RunValidation() == false)
        {
            return;
        }
    }

    private bool RunValidation()
    {
        if (_customValidation == null)
        {
            throw new InvalidOperationException("Custom validation variable is not set during validation.");
        }

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (string.IsNullOrWhiteSpace(_form.Name))
        {
            errors.Add(nameof(_form.Name), new List<string>() {
                "Name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(_form.BasePortions))
        {
            errors.Add(nameof(_form.BasePortions), new List<string>() {
                "Amount of portions is required."
            });
        }
        else if (int.TryParse(_form.BasePortions, out int basePortions) == false)
        {
            errors.Add(nameof(_form.BasePortions), new List<string>() {
                "Portions must only include numbers."
            });
        }
        else if (basePortions <= 0)
        {
            errors.Add(nameof(_form.BasePortions), new List<string>() {
                "Portions must be a positive number."
            });
        }

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        return true;
    }

    private class Form
    {
        public string Name { get; set; } = string.Empty;
        public string BasePortions { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }
}
