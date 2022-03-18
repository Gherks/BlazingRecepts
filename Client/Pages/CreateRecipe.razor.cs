using BlazingRecept.Client.Utilities;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class CreateRecipe : ComponentBase
{
    private readonly Form _form = new();

    private CustomValidation? _customValidation;

    private void ValidFormSubmitted()
    {
        if (RunValidation() == false)
        {
            return;
        }

        //ParseForm();

        //await BaseDataService.SaveAsync(_baseDataDto);
        //await SignaturesService.SaveAsync(_signaturesDto);

        //GotoNextStep();
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
        public string InstructionsPreview => Markdown.ToHtml(Instructions);
    }
}
