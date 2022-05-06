using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;

public partial class UpdateIngredientMeasurementModal : PageComponentBase
{
    private static readonly string _editFormId = "UpdateIngredientMeasurementModalEditForm";

    private HxModal? _modal;
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

    public async Task Open(IngredientMeasurementDto? ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(ingredientMeasurementDto, "Edit ingredient measurement modal cannot be opened because it has no ingredient to edit.");
        Contracts.LogAndThrowWhenNull(_modal, "Edit ingredient measurement modal cannot be opened because modal has not been set.");

        _editIngredientDto = ingredientMeasurementDto.IngredientDto;

        _form = new()
        {
            Measurement = ingredientMeasurementDto.Measurement,
            MeasurementUnit = ingredientMeasurementDto.MeasurementUnit,
            Grams = ingredientMeasurementDto.Grams,
            Note = ingredientMeasurementDto.Note,
        };

        _editContext = new(_form);
        await _modal.ShowAsync();
    }

    private async Task HandleCancel()
    {
        Contracts.LogAndThrowWhenNull(_modal, "Edit ingredient measurement modal cannot be closed because modal has not been set.");

        await _modal.HideAsync();
    }

    private async Task HandleValidFormSubmitted()
    {
        Contracts.LogAndThrowWhenNull(RecipeWorkbench, "Cannot update ingredient measurement because recipe workbench page reference has not been set.");
        Contracts.LogAndThrowWhenNull(_editIngredientDto, "Cannot update ingredient measurement because editing ingredient dto has not been set.");
        Contracts.LogAndThrowWhenNull(_modal, "Cannot update ingredient measurement because modal has not been set.");

        if (Validate())
        {
            IngredientMeasurementDto? ingredientMeasurementDto = RecipeWorkbench.ContainedIngredientMeasurements
                .FirstOrDefault(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == _editIngredientDto.Id);

            Contracts.LogAndThrowWhenNull(ingredientMeasurementDto, "Failed to find ingredient measurement that was expected to exist in edited recipe.");

            EditIngredientMeasurementDtoWithForm(ingredientMeasurementDto);

            RecipeWorkbench.Refresh();
            await _modal.HideAsync();
        }
    }

    private bool Validate()
    {
        Contracts.LogAndThrowWhenNull(_customValidation, "Custom validation is not set during edit ingredient measurement modal validation.");
        Contracts.LogAndThrowWhenNull(_form, "Form is not set during edit ingredient measurement modal validation.");

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
        Contracts.LogAndThrowWhenNull(_form.Measurement, "Cannot create ingredient measurement dto from form because measurement in form has not been set.");
        Contracts.LogAndThrowWhenNull(_form.Grams, "Cannot create ingredient measurement dto from form because grams in form has not been set.");

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
