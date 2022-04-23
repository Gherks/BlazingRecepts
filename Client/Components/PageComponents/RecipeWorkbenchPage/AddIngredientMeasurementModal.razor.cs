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

public partial class AddIngredientMeasurementModal : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "AddIngredientMeasurementModal";
    private static readonly string _editFormId = "AddIngredientMeasurementModalEditForm";

    private Modal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    private Form _form = new();

    [CascadingParameter]
    public RecipeWorkbench? RecipeWorkbench { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        _editContext = new(_form);

        IsLoading = false;
    }

    public void Open()
    {
        if (_modal == null)
        {
            string errorMessage = "Add ingredient measurement modal cannot be opened because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _form = new();
        _editContext = new(_form);
        _modal.Open();
    }

    private void HandleCancel()
    {
        if (_modal == null)
        {
            string errorMessage = "Add ingredient measurement modal cannot be closed because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _modal.Close();
    }

    private void HandleValidFormSubmitted()
    {
        if (_modal == null)
        {
            string errorMessage = "Add ingredient measurement modal form cannot be validated because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeWorkbench == null)
        {
            string errorMessage = "Add ingredient measurement modal form cannot be validated because RecipeWorkbench page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (Validate())
        {
            IngredientMeasurementDto newIngredientMeasurementDto = CreateIngredientMeasurementDtoFromForm();
            RecipeWorkbench.ContainedIngredientMeasurements.Add(newIngredientMeasurementDto);

            RecipeWorkbench.Refresh();
            _modal.Close();
        }
    }

    private bool Validate()
    {
        if (_customValidation == null)
        {
            string errorMessage = "Custom validation is not set during add ingredient measurement modal validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form == null)
        {
            string errorMessage = "Form is not set during add ingredient measurement modal validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (_form.IngredientDto == null)
        {
            errors.Add(nameof(_form.IngredientDto), new List<string>() {
                "Ingrediens måste anges."
            });
        }
        else if (IngredientAlreadyAdded(_form.IngredientDto))
        {
            errors.Add(nameof(_form.IngredientDto), new List<string>() {
                "Ingrediens har redan tillagts."
            });
        }

        if (_form.MeasurementUnit == MeasurementUnit.Unassigned)
        {
            errors.Add(nameof(_form.MeasurementUnit), new List<string>() {
                "Mätningstyp måste anges."
            });
        }

        InputValidation.ValidateNullableDouble(_form.Measurement, nameof(_form.Measurement), "Gram", errors);
        InputValidation.ValidateNullableDouble(_form.Grams, nameof(_form.Grams), "Gram", errors);

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        StateHasChanged();
        return true;
    }

    private IngredientMeasurementDto CreateIngredientMeasurementDtoFromForm()
    {
        //if (RecipeWorkbench == null)
        //{
        //    string errorMessage = "Add ingredient measurement modal form cannot be validated because RecipeWorkbench page reference has not been set.";
        //    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
        //    throw new InvalidOperationException(errorMessage);
        //}

        int sortOrder = _form.SortOrder;

        // huh?
        //if (sortOrder >= 0)
        //{
        //    sortOrder = RecipeWorkbench.ContainedIngredientMeasurements.Count;
        //}

        if (_form.IngredientDto == null)
        {
            string errorMessage = "Cannot create ingredient measurement dto from form because ingredient dto in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.Measurement == null)
        {
            string errorMessage = "Cannot create ingredient measurement dto from form because measurement in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.Grams == null)
        {
            string errorMessage = "Cannot create ingredient measurement dto from form because grams in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return new()
        {
            IngredientDto = _form.IngredientDto,
            Measurement = _form.Measurement.Value,
            MeasurementUnit = _form.MeasurementUnit,
            Grams = _form.Grams.Value,
            Note = _form.Note,
            SortOrder = sortOrder
        };
    }

    private async Task<IEnumerable<IngredientDto>> SearchForIngredients(string searchTerm)
    {
        if (RecipeWorkbench == null)
        {
            string errorMessage = "RecipeWorkbench page reference has not been set during ingredient input search procedure.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeWorkbench.Ingredients == null)
        {
            string errorMessage = "RecipeWorkbench page reference contains no ingredients during ingredient input search procedure.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        List<IngredientDto> foundIngredients = RecipeWorkbench.Ingredients.Where(ingredientDto => ingredientDto.Name.ToLower().Contains(searchTerm.ToLower())).ToList();

        if (foundIngredients.Count == 1)
        {
            _form.IngredientDto = foundIngredients.First();
        }

        return await Task.FromResult(foundIngredients);
    }

    private bool IngredientAlreadyAdded(IngredientDto IngredientDto)
    {
        if (RecipeWorkbench == null)
        {
            string errorMessage = "RecipeWorkbench page reference has not been set before checking for ingredient duplicates in edited recipe.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return RecipeWorkbench.ContainedIngredientMeasurements.Any(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == IngredientDto.Id);
    }

    private class Form
    {
        public IngredientDto? IngredientDto { get; set; } = null;
        public double? Measurement { get; set; } = null;
        public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Unassigned;
        public double? Grams { get; set; } = null;
        public string Note { get; set; } = string.Empty;
        public int SortOrder { get; set; } = -1;
    }
}
