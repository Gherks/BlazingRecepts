using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Serilog;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;

public partial class EditIngredientMeasurementModal : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "EditIngredientMeasurementModal";
    private static readonly string _editFormId = "EditIngredientMeasurementModalEditForm";

    private Modal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    private Form _form = new();
    private IngredientDto? _editIngredientDto;

    [CascadingParameter]
    public RecipeWorkbench? RecipeWorkbench { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        _editContext = new(_form);

        IsLoading = false;
    }

    public void Open(IngredientMeasurementDto? ingredientMeasurementDto)
    {
        if (_modal == null)
        {
            string errorMessage = "Edit ingredient measurement modal cannot be opened because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ingredientMeasurementDto == null)
        {
            string errorMessage = "Edit ingredient measurement modal cannot be opened because it has no ingredient to edit.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(ingredientMeasurementDto), errorMessage);
        }

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
        if (_modal == null)
        {
            string errorMessage = "Edit ingredient measurement modal cannot be closed because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _modal.Close();
    }

    private void HandleValidFormSubmitted()
    {
        if (_modal == null)
        {
            string errorMessage = "Edit ingredient measurement modal form cannot be validated because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeWorkbench == null)
        {
            string errorMessage = "Edit ingredient measurement modal form cannot be validated because RecipeWorkbench page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _form.Measurement = _form.Measurement.Replace(',', '.');
        _form.Grams = _form.Grams.Replace(',', '.');

        if (Validate())
        {
            if (RecipeWorkbench == null)
            {
                string errorMessage = ".";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            if (_editIngredientDto == null)
            {
                string errorMessage = ".";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            IngredientMeasurementDto? ingredientMeasurementDto = RecipeWorkbench.ContainedIngredientMeasurements
                .FirstOrDefault(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == _editIngredientDto.Id);

            if (ingredientMeasurementDto == null)
            {
                string errorMessage = "Failed to find ingredient measurement that was expected to exist in edited recipe.";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

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
        if (_customValidation == null)
        {
            string errorMessage = "Custom validation is not set during edit ingredient measurement modal validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form == null)
        {
            string errorMessage = "Form is not set during edit ingredient measurement modal validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (_form.MeasurementUnit == MeasurementUnit.Unassigned)
        {
            errors.Add(nameof(_form.MeasurementUnit), new List<string>() {
                "Mätningstyp måste anges."
            });
        }

        InputValidation.ValidateStringToDouble(_form.Measurement, nameof(_form.Measurement), "Mätning", errors);
        InputValidation.ValidateStringToDouble(_form.Grams, nameof(_form.Grams), "Gram", errors);

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
