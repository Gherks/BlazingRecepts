using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;

public partial class EditIngredientMeasurementModal : ComponentBase
{
    private readonly string _editFormId = "EditIngredientMeasurementModalEditForm";

    private Modal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    private Form _form = new();
    private IngredientDto? _editIngredientDto;

    [CascadingParameter]
    public RecipeWorkbench? RecipeWorkbench { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _editContext = new(_form);
    }

    public void Open(IngredientMeasurementDto? ingredientMeasurementDto)
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be opened because it has not been set.");
        if (ingredientMeasurementDto == null) throw new InvalidOperationException("Modal cannot be opened because it has no ingredient to edit.");

        _editIngredientDto = ingredientMeasurementDto.IngredientDto;

        _form = new()
        {
            Measurement = ingredientMeasurementDto.Measurement.ToString(),
            MeasurementUnit = ingredientMeasurementDto.MeasurementUnit,
            Grams = ingredientMeasurementDto.Grams.ToString(),
            Note = ingredientMeasurementDto.Note,
        };

        _editContext = new(_form);
        _modal.Open();
    }

    private void HandleCancel()
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be closed because it has not been set.");

        _modal.Close();
    }

    private void HandleValidFormSubmitted()
    {
        if (_modal == null) throw new InvalidOperationException();
        if (RecipeWorkbench == null) throw new InvalidOperationException();

        _form.Measurement = _form.Measurement.Replace(',', '.');
        _form.Grams = _form.Grams.Replace(',', '.');

        if (Validate())
        {
            if (RecipeWorkbench == null) throw new InvalidOperationException();
            if (_editIngredientDto == null) throw new InvalidOperationException();

            IngredientMeasurementDto? ingredientMeasurementDto = RecipeWorkbench.ContainedIngredientMeasurements
                .FirstOrDefault(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == _editIngredientDto.Id);

            if (ingredientMeasurementDto == null) throw new InvalidOperationException("Failed to find ingredient that was expected to exist edited recipe.");

            ingredientMeasurementDto.Measurement = Convert.ToDouble(_form.Measurement);
            ingredientMeasurementDto.MeasurementUnit = _form.MeasurementUnit;
            ingredientMeasurementDto.Grams = Convert.ToDouble(_form.Grams);
            ingredientMeasurementDto.Note = _form.Note;

            RecipeWorkbench.Refresh();
            _modal.Close();
        }
    }

    private bool Validate()
    {
        if (_customValidation == null) throw new InvalidOperationException("Custom validation is not set during validation.");
        if (_form == null) throw new InvalidOperationException("Form is not set during edit ingredient modal validation.");

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (string.IsNullOrWhiteSpace(_form.Measurement))
        {
            errors.Add(nameof(_form.Measurement), new List<string>() {
                "Measurement is required."
            });
        }
        else if (double.TryParse(_form.Measurement, out double grams) == false)
        {
            errors.Add(nameof(_form.Measurement), new List<string>() {
                "Measurement must only include numbers."
            });
        }
        else if (grams <= 0)
        {
            errors.Add(nameof(_form.Measurement), new List<string>() {
                "Measurement cannot be less than zero."
            });
        }

        if (_form.MeasurementUnit == MeasurementUnit.Unassigned)
        {
            errors.Add(nameof(_form.MeasurementUnit), new List<string>() {
                "Measurement unit is required."
            });
        }

        if (string.IsNullOrWhiteSpace(_form.Grams))
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
                "Grams is required."
            });
        }
        else if (double.TryParse(_form.Grams, out double grams) == false)
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
                "Grams must only include numbers."
            });
        }
        else if (grams <= 0)
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
                "Grams cannot be less than zero."
            });
        }

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        StateHasChanged();
        return true;
    }

    public class Form
    {
        public string Measurement { get; set; } = string.Empty;
        public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Unassigned;
        public string Grams { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
