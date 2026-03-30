using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Extensions;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Contract;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazingRecept.Client.Components.PageComponents.DailyIntakePage;

public partial class AddDailyIntakeEntryModal : PageComponentBase
{
    private static readonly string _editFormId = "AddDailyIntakeEntryModalEditForm";

    private HxModal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;
    private HxAutosuggest<string, string>? _productName;
    private ProcessingButton? _processingButtonSubmit;

    private DailyIntakeTable? _dailyIntakeTable;
    private Guid _collectionId;
    private Form _form = new();

    [CascadingParameter]
    protected internal DailyIntake? DailyIntakePage { get; private set; }

    [Inject]
    protected internal IDailyIntakeEntryService? DailyIntakeEntryService { get; private set; }

    [Inject]
    protected internal IHxMessengerService? MessengerService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        _editContext = new(_form);

        IsLoading = false;
    }

    public async Task Open(DailyIntakeTable dailyIntakeTable, Guid collectionId)
    {
        Contracts.LogAndThrowWhenNull(_modal, "Add ingredient measurement modal cannot be opened because modal has not been set.");

        _dailyIntakeTable = dailyIntakeTable;
        _collectionId = collectionId;
        _form = new();

        _editContext = new(_form);
        await _modal.ShowAsync();
    }

    private async Task HandleOnShown()
    {
        Contracts.LogAndThrowWhenNull(_productName, "Add ingredient measurement modal cannot be opened because modal has not been set.");

        await _productName.FocusAsync();
    }

    private async Task HandleCancel()
    {
        Contracts.LogAndThrowWhenNull(_modal, "Add ingredient measurement modal cannot be closed because modal has not been set.");

        await _modal.HideAsync();
    }

    private async Task<AutosuggestDataProviderResult<string>> ProvideSuggestionsToProductNameAutosuggest(AutosuggestDataProviderRequest request)
    {
        Contracts.LogAndThrowWhenNull(DailyIntakePage, "DailyIntake page reference has not been set during ingredient input search procedure.");
        Contracts.LogAndThrowWhenNull(DailyIntakePage.Ingredients, "DailyIntake page reference contains no ingredients during daily intake ingredient input search procedure.");
        Contracts.LogAndThrowWhenNull(DailyIntakePage.Recipes, "DailyIntake page reference contains no recipes during daily intake recipe input search procedure.");

        List<IngredientDto> foundIngredients = DailyIntakePage.Ingredients
            .Where(ingredientDto => ingredientDto.Name?.Contains(request.UserInput, StringComparison.OrdinalIgnoreCase) ?? false)
            .OrderBy(ingredientDto => ingredientDto.Name)
            .ToList();

        List<RecipeDto> foundRecipes = DailyIntakePage.Recipes
            .Where(recipeDto => recipeDto.Name?.Contains(request.UserInput, StringComparison.OrdinalIgnoreCase) ?? false)
            .OrderBy(recipeDto => recipeDto.Name)
            .ToList();

        List<string> foundProductNames = new();

        foreach (IngredientDto ingredientDto in foundIngredients)
        {
            foundProductNames.Add(ingredientDto.Name);
        }

        foreach (RecipeDto recipeDto in foundRecipes)
        {
            foundProductNames.Add(recipeDto.Name);
        }

        return await Task.FromResult(new AutosuggestDataProviderResult<string>
        {
            Data = foundProductNames.ToList()
        });
    }

    private async Task HandleValidFormSubmitted()
    {
        Contracts.LogAndThrowWhenNull(_processingButtonSubmit, "Add ingredient measurement modal form cannot be validated because processing button has not been set.");

        _processingButtonSubmit.IsProcessing = true;

        Contracts.LogAndThrowWhenNull(_modal, "Add ingredient measurement modal form cannot be validated because modal has not been set.");
        Contracts.LogAndThrowWhenNull(DailyIntakePage, "Daily intake page reference is not available during form validation.");
        Contracts.LogAndThrowWhenNull(DailyIntakeEntryService, "Daily intake entry service is not available during form validation.");
        Contracts.LogAndThrowWhenNull(_dailyIntakeTable, "Daily intake entry table is not available during form validation.");

        if (Validate())
        {
            DailyIntakeEntryDto newDailyIntakeEntryDto = CreateDailyIntakeEntryDtoFromForm();
            DailyIntakeEntryDto? savedDailyIntakeEntryDto = await DailyIntakeEntryService.SaveAsync(newDailyIntakeEntryDto);

            if (savedDailyIntakeEntryDto != null && savedDailyIntakeEntryDto.Id != Guid.Empty)
            {
                List<DailyIntakeEntryDto> updatedDailyIntakeEntryDtos = DailyIntakePage.UpsertDailyIntakeEntryIntoCollection(savedDailyIntakeEntryDto);
                _dailyIntakeTable.ConstructCheckableDailyIntakeEntryList(updatedDailyIntakeEntryDtos);
                _dailyIntakeTable = null;

                MessengerService.AddSuccess("Dagligt intag", "Post för dagligt intag tillagd!");
                await _modal.HideAsync();
            }
            else
            {
                MessengerService.AddError("Dagligt intag", "Kunde ej lägga till post för dagligt intag.");
            }
        }

        _processingButtonSubmit.IsProcessing = false;
        DailyIntakePage.Refresh();
    }

    private bool Validate()
    {
        Contracts.LogAndThrowWhenNull(_customValidation, "Custom validation object is not available during validation.");

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (string.IsNullOrWhiteSpace(_form.ProductName))
        {
            errors.Add(nameof(_form.ProductName), new List<string>() {
                "Namn måste anges."
            });
        }

        InputValidation.ValidateNullableDouble(_form.Amount, nameof(_form.Amount), "Mängd", errors);

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        return true;
    }

    private DailyIntakeEntryDto CreateDailyIntakeEntryDtoFromForm()
    {
        Contracts.LogAndThrowWhenNull(DailyIntakePage, "Cannot create daily intake entry dto from form because daily intake page reference has not been set.");
        Contracts.LogAndThrowWhenNull(_form.ProductName, "Cannot create daily intake entry dto from form because name in form has not been set.");
        Contracts.LogAndThrowWhenNull(_form.Amount, "Cannot create daily intake entry dto from form because amount in form has not been set.");

        DailyIntakeEntryDto dailyIntakeEntryDto = new()
        {
            ProductName = _form.ProductName.Trim(),
            Amount = _form.Amount.Value,
            SortOrder = DailyIntakePage.DailyIntakeEntryDtoCollections[_collectionId].Count,
            CollectionId = _collectionId
        };

        return dailyIntakeEntryDto;
    }

    private class Form
    {
        public string? ProductName { get; set; } = null;
        public double? Amount { get; set; } = null;
    }
}
