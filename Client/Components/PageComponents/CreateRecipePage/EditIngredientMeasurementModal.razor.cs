using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Components.PageComponents.CreateRecipePage;

public partial class EditIngredientMeasurementModal : ComponentBase
{
    private readonly string _editFormId = "EditIngredientMeasurementModalEditForm";

    private Modal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    private Form _form = new();
    private IngredientDto? _editIngredientDto;

    [CascadingParameter]
    public CreateRecipe? CreateRecipe { get; set; }

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
            Measurement = ingredientMeasurementDto.Measurement,
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
        if (CreateRecipe == null) throw new InvalidOperationException();

        if (Validate())
        {
            if (CreateRecipe == null) throw new InvalidOperationException();
            if (_editIngredientDto == null) throw new InvalidOperationException();

            IngredientMeasurementDto? ingredientMeasurementDto = CreateRecipe.ContainedIngredientMeasurements
                .FirstOrDefault(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == _editIngredientDto.Id);

            if (ingredientMeasurementDto == null) throw new InvalidOperationException("Failed to find ingredient that was expected to exist edited recipe.");

            ingredientMeasurementDto.Measurement = _form.Measurement;
            ingredientMeasurementDto.MeasurementUnit = _form.MeasurementUnit;
            ingredientMeasurementDto.Grams = Convert.ToInt32(_form.Grams);
            ingredientMeasurementDto.Note = _form.Note;

            CreateRecipe.Refresh();
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

        if (string.IsNullOrWhiteSpace(_form.Grams))
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
                "Grams is required."
            });
        }
        else if (int.TryParse(_form.Grams, out int basePortions) == false)
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
                "Grams must only include numbers."
            });
        }
        else if (basePortions <= 0)
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
                "Grams must be a positive number."
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
        public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Gram;
        public string Grams { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
