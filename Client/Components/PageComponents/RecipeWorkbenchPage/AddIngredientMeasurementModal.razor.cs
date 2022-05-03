using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web.Bootstrap;
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

    private HxModal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    private HxAutosuggest<IngredientDto, IngredientDto>? _name;
    private Form _form = new();

    [CascadingParameter]
    protected internal RecipeWorkbench? RecipeWorkbench { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        _editContext = new(_form);

        IsLoading = false;
    }

    public async Task Open()
    {
        if (_modal == null)
        {
            const string errorMessage = "Add ingredient measurement modal cannot be opened because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _form = new();
        _editContext = new(_form);
        await _modal.ShowAsync();
    }

    private async Task HandleOnShown()
    {
        if (_name == null)
        {
            const string errorMessage = "Input for name has not been set before showing the modal.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await _name.FocusAsync();
    }

    private async Task HandleCancel()
    {
        if (_modal == null)
        {
            const string errorMessage = "Add ingredient measurement modal cannot be closed because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await _modal.HideAsync();
    }

    private async Task<AutosuggestDataProviderResult<IngredientDto>> ProvideSuggestionsToNameAutosuggest(AutosuggestDataProviderRequest request)
    {
        if (RecipeWorkbench == null)
        {
            const string errorMessage = "Cannot provide ingredient suggestions to name autosuggest because recipe workbench page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeWorkbench.Ingredients == null)
        {
            const string errorMessage = "Cannot provide ingredient suggestions to name autosuggest because recipe workbench ingredient list has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return await Task.FromResult(new AutosuggestDataProviderResult<IngredientDto>
        {
            Data = RecipeWorkbench.Ingredients
                .Where(ingredientDto => ingredientDto.Name?.Contains(request.UserInput, StringComparison.CurrentCultureIgnoreCase) ?? false)
                .OrderBy(ingredientDto => ingredientDto.Name)
                .ToList()
        });
    }

    private string ResolvePickedAutosuggestItemToTextInNameAutosuggest(IngredientDto ingredientDto)
    {
        return ingredientDto.Name;
    }

    private async Task HandleValidFormSubmitted()
    {
        if (_modal == null)
        {
            const string errorMessage = "Cannot add ingredient measurement because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeWorkbench == null)
        {
            const string errorMessage = "Cannot add ingredient measurement because recipe workbench page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (Validate())
        {
            IngredientMeasurementDto newIngredientMeasurementDto = CreateIngredientMeasurementDtoFromForm();
            RecipeWorkbench.ContainedIngredientMeasurements.Add(newIngredientMeasurementDto);

            RecipeWorkbench.Refresh();
            await _modal.HideAsync();
        }
    }

    private bool Validate()
    {
        if (_customValidation == null)
        {
            const string errorMessage = "Custom validation is not set during add ingredient measurement modal validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form == null)
        {
            const string errorMessage = "Form is not set during add ingredient measurement modal validation.";
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
        if (RecipeWorkbench == null)
        {
            const string errorMessage = "Cannot create ingredient measurement dto from form because recipe workbench page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.IngredientDto == null)
        {
            const string errorMessage = "Cannot create ingredient measurement dto from form because ingredient dto in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

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

        return new()
        {
            IngredientDto = _form.IngredientDto,
            Measurement = _form.Measurement.Value,
            MeasurementUnit = _form.MeasurementUnit,
            Grams = _form.Grams.Value,
            Note = _form.Note,
            SortOrder = RecipeWorkbench.ContainedIngredientMeasurements.Count
        };
    }

    private bool IngredientAlreadyAdded(IngredientDto IngredientDto)
    {
        if (RecipeWorkbench == null)
        {
            const string errorMessage = "RecipeWorkbench page reference has not been set before checking for ingredient duplicates in edited recipe.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return RecipeWorkbench.ContainedIngredientMeasurements.Any(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == IngredientDto.Id);
    }

    private class Form
    {
        public IngredientDto? IngredientDto { get; set; } = null;
        public Guid? IngredientId { get; set; } = null;
        public double? Measurement { get; set; } = null;
        public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Unassigned;
        public double? Grams { get; set; } = null;
        public string Note { get; set; } = string.Empty;
    }
}
