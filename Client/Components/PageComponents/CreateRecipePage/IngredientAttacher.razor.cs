using BlazingRecept.Client.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazingRecept.Client.Components.PageComponents.CreateRecipePage;

public partial class IngredientAttacher : ComponentBase
{
    private readonly string _editFormId = "IngredientAttacherEditForm";

    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    List<IngredientRow> _ingredientRows = new List<IngredientRow>();

    public IngredientAttacher()
    {
        AddIngredientRow();
    }

    private void AddIngredientRow()
    {
        _ingredientRows.Add(new());
        _editContext = new(_ingredientRows);
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


        //if (string.IsNullOrWhiteSpace(_form.Name))
        //{
        //    errors.Add(nameof(_form.Name), new List<string>() {
        //        "Name is required."
        //    });
        //}

        //if (string.IsNullOrWhiteSpace(_form.Grams))
        //{
        //    errors.Add(nameof(_form.Grams), new List<string>() {
        //        "Amount of portions is required."
        //    });
        //}
        //else if (int.TryParse(_form.Grams, out int basePortions) == false)
        //{
        //    errors.Add(nameof(_form.Grams), new List<string>() {
        //        "Portions must only include numbers."
        //    });
        //}
        //else if (basePortions <= 0)
        //{
        //    errors.Add(nameof(_form.Grams), new List<string>() {
        //        "Portions must be a positive number."
        //    });
        //}

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        return true;
    }

    private class IngredientRow
    {
        public string Name { get; set; } = string.Empty;
        public string Grams { get; set; } = string.Empty;
    }
}
