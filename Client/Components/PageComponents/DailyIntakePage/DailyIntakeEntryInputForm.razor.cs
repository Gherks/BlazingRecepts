using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Extensions;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.DailyIntakePage;

public partial class DailyIntakeEntryInputForm : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "DailyIntakeEntryInputForm";

    private Form _form = new();

    private CustomValidation? _customValidation;

    [CascadingParameter]
    protected internal DailyIntake? DailyIntakePage { get; private set; }

    [Parameter]
    public Guid CollectionId { get; set; }

    [Inject]
    public IDailyIntakeEntryService? DailyIntakeEntryService { get; private set; }

    [Inject]
    public IToastService? ToastService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        IsLoading = false;
    }

    private async Task<IEnumerable<string>> SearchForRecipeOrIngredient(string searchTerm)
    {
        if (DailyIntakePage == null)
        {
            string errorMessage = "DailyIntake page reference has not been set during ingredient input search procedure.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakePage.Ingredients == null)
        {
            string errorMessage = "DailyIntake page reference contains no ingredients during daily intake ingredient input search procedure.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakePage.Recipes == null)
        {
            string errorMessage = "DailyIntake page reference contains no recipes during daily intake recipe input search procedure.";
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
        if (DailyIntakePage == null)
        {
            string errorMessage = "Daily intake page reference is not available during form validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakeEntryService == null)
        {
            string errorMessage = "Daily intake entry service is not available during form validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ToastService == null)
        {
            string errorMessage = "Toast service is not available during form validation.";
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
                ToastService.ShowSuccess("Post för dagligt intag tillagd!");

                StateHasChanged();
            }
            else
            {
                ToastService.ShowError("Kunde ej lägga till post för dagligt intag.");
            }
        }
    }

    private bool Validate()
    {
        if (_customValidation == null)
        {
            string errorMessage = "Custom validation object is not available during validation.";
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

        InputValidation.ValidateStringToDouble(_form.Amount, nameof(_form.Amount), "Mängd", errors);

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
            string errorMessage = "Daily intake page reference is not available during daily intake entry construction.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        DailyIntakeEntryDto dailyIntakeEntryDto = new()
        {
            ProductName = _form.ProductName.Trim(),
            Amount = double.Parse(_form.Amount),
            CollectionId = CollectionId
        };

        dailyIntakeEntryDto.LoadFromProductListsByName(DailyIntakePage.Ingredients, DailyIntakePage.Recipes);

        return dailyIntakeEntryDto;
    }

    private class Form
    {
        public string ProductName { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
    }
}
