using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Serilog;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;

public partial class UpdateIngredientMeasurementModal : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "UpdateIngredientMeasurementModal";
    private static readonly string _editFormId = "UpdateIngredientMeasurementModalEditForm";

    private Modal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    private Form _form = new();
    private IngredientDto? _editIngredientDto;

    [CascadingParameter]
    protected internal RecipeWorkbench? RecipeWorkbench { get; private set; }

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
            const string errorMessage = "Edit ingredient measurement modal cannot be opened because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ingredientMeasurementDto == null)
        {
            const string errorMessage = "Edit ingredient measurement modal cannot be opened because it has no ingredient to edit.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(ingredientMeasurementDto), errorMessage);
        }

        _editIngredientDto = ingredientMeasurementDto.IngredientDto;

        _form = new()
        {
            Measurement = ingredientMeasurementDto.Measurement,
            MeasurementUnit = ingredientMeasurementDto.MeasurementUnit,
            Grams = ingredientMeasurementDto.Grams,
            Note = ingredientMeasurementDto.Note,
        };

        _editContext = new(_form);
        _modal.Open();
    }

    private void HandleCancel()
    {
        if (_modal == null)
        {
            const string errorMessage = "Edit ingredient measurement modal cannot be closed because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _modal.Close();
    }

    private void HandleValidFormSubmitted()
    {
        if (_modal == null)
        {
            const string errorMessage = "Edit ingredient measurement modal form cannot be validated because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeWorkbench == null)
        {
            const string errorMessage = "Edit ingredient measurement modal form cannot be validated because RecipeWorkbench page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (Validate())
        {
            if (RecipeWorkbench == null)
            {
                const string errorMessage = ".";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            if (_editIngredientDto == null)
            {
                const string errorMessage = ".";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            IngredientMeasurementDto? ingredientMeasurementDto = RecipeWorkbench.ContainedIngredientMeasurements
                .FirstOrDefault(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == _editIngredientDto.Id);

            if (ingredientMeasurementDto == null)
            {
                const string errorMessage = "Failed to find ingredient measurement that was expected to exist in edited recipe.";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            EditIngredientMeasurementDtoWithForm(ingredientMeasurementDto);

            RecipeWorkbench.Refresh();
            _modal.Close();
        }
    }

    private bool Validate()
    {
        if (_customValidation == null)
        {
            const string errorMessage = "Custom validation is not set during edit ingredient measurement modal validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form == null)
        {
            const string errorMessage = "Form is not set during edit ingredient measurement modal validation.";
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

        InputValidation.ValidateNullableDouble(_form.Measurement, nameof(_form.Measurement), "Mätning", errors);
        InputValidation.ValidateNullableDouble(_form.Grams, nameof(_form.Grams), "Gram", errors);

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        StateHasChanged();
        return true;
    }

    private void EditIngredientMeasurementDtoWithForm(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (_form.Measurement == null)
        {
            const string errorMessage = "Cannot create ingredient measurement dto from form because measurement in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.Grams == null)
        {
            const string errorMessage = "Cannot create ingredient measurement dto from form because grams in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        ingredientMeasurementDto.Measurement = _form.Measurement.Value;
        ingredientMeasurementDto.MeasurementUnit = _form.MeasurementUnit;
        ingredientMeasurementDto.Grams = _form.Grams.Value;
        ingredientMeasurementDto.Note = _form.Note;
    }

    private class Form
    {
        public double? Measurement { get; set; } = null;
        public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Unassigned;
        public double? Grams { get; set; } = null;
        public string Note { get; set; } = string.Empty;
    }
}
