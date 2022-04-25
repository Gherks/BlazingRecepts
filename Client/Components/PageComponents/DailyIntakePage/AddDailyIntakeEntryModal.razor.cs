using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Extensions;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.DailyIntakePage;

public partial class AddDailyIntakeEntryModal : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "AddDailyIntakeEntryModal";
    private static readonly string _editFormId = "AddDailyIntakeEntryModalEditForm";

    private Modal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

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

    public void Open(Guid collectionId)
    {
        if (_modal == null)
        {
            const string errorMessage = "Add ingredient measurement modal cannot be opened because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _collectionId = collectionId;
        _form = new();

        _editContext = new(_form);
        _modal.Open();
    }

    private void HandleCancel()
    {
        if (_modal == null)
        {
            const string errorMessage = "Add ingredient measurement modal cannot be closed because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _modal.Close();
    }

    private async Task<IEnumerable<string>> SearchForRecipeOrIngredient(string searchTerm)
    {
        if (DailyIntakePage == null)
        {
            const string errorMessage = "DailyIntake page reference has not been set during ingredient input search procedure.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakePage.Ingredients == null)
        {
            const string errorMessage = "DailyIntake page reference contains no ingredients during daily intake ingredient input search procedure.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakePage.Recipes == null)
        {
            const string errorMessage = "DailyIntake page reference contains no recipes during daily intake recipe input search procedure.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        List<IngredientDto> foundIngredients = DailyIntakePage.Ingredients.Where(ingredientDto => ingredientDto.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
        List<RecipeDto> foundRecipes = DailyIntakePage.Recipes.Where(recipeDto => recipeDto.Name.ToLower().Contains(searchTerm.ToLower())).ToList();

        List<string> foundProductNames = new();

        foreach (IngredientDto ingredientDto in foundIngredients)
        {
            foundProductNames.Add(ingredientDto.Name);
        }

        foreach (RecipeDto recipeDto in foundRecipes)
        {
            foundProductNames.Add(recipeDto.Name);
        }

        if (foundProductNames.Count == 1)
        {
            _form.ProductName = foundProductNames.First();
        }

        return await Task.FromResult(foundProductNames);
    }

    private async Task HandleValidFormSubmitted()
    {
        if (_modal == null)
        {
            const string errorMessage = "Add ingredient measurement modal form cannot be validated because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakePage == null)
        {
            const string errorMessage = "Daily intake page reference is not available during form validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakeEntryService == null)
        {
            const string errorMessage = "Daily intake entry service is not available during form validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (Validate())
        {
            DailyIntakeEntryDto newDailyIntakeEntryDto = CreateDailyIntakeEntryDtoFromForm();
            DailyIntakeEntryDto? savedDailyIntakeEntryDto = await DailyIntakeEntryService.SaveAsync(newDailyIntakeEntryDto);

            if (savedDailyIntakeEntryDto != null && savedDailyIntakeEntryDto.Id != Guid.Empty)
            {
                DailyIntakePage.UpsertDailyIntakeEntryIntoCollection(savedDailyIntakeEntryDto);

                _form = new();
                MessengerService.AddSuccess("Dagligt intag", "Post för dagligt intag tillagd!");

                StateHasChanged();
                _modal.Close();
            }
            else
            {
                MessengerService.AddError("Dagligt intag", "Kunde ej lägga till post för dagligt intag.");
            }
        }
    }

    private bool Validate()
    {
        if (_customValidation == null)
        {
            const string errorMessage = "Custom validation object is not available during validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

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
        if (DailyIntakePage == null)
        {
            const string errorMessage = "Cannot create daily intake entry dto from form because daily intake page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.ProductName == null)
        {
            const string errorMessage = "Cannot create daily intake entry dto from form because name in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.Amount == null)
        {
            const string errorMessage = "Cannot create daily intake entry dto from form because amount in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        DailyIntakeEntryDto dailyIntakeEntryDto = new()
        {
            ProductName = _form.ProductName.Trim(),
            Amount = _form.Amount.Value,
            SortOrder = DailyIntakePage.DailyIntakeEntryDtoCollections[_collectionId].Count,
            CollectionId = _collectionId
        };

        dailyIntakeEntryDto.LoadFromProductListsByName(DailyIntakePage.Ingredients, DailyIntakePage.Recipes);

        return dailyIntakeEntryDto;
    }

    private class Form
    {
        public string? ProductName { get; set; } = null;
        public double? Amount { get; set; } = null;
    }
}
