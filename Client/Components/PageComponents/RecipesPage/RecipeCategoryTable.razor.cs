using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.RecipesPage;

public partial class RecipeCategoryTable : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipeCategoryTable";

    private IReadOnlyList<RecipeDto>? _recipeDtos = new List<RecipeDto>();

    private Dictionary<char, List<RecipeDto>> _recipeCategories = new();

    [Inject]
    protected internal IRecipeService? RecipeService { get; private set; }

    [Inject]
    protected internal NavigationManager? NavigationManager { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        if (RecipeService == null)
        {
            string errorMessage = "Cannot initialize RecipeCategoryTable because recipe service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _recipeDtos = await RecipeService.GetAllAsync();

        CategorizeByName();

        IsLoading = false;
    }

    private void CategorizeByName()
    {
        if (_recipeDtos == null)
        {
            string errorMessage = "Cannot categorize recipes because there are no recipes to categorize.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _recipeCategories.Clear();

        foreach (RecipeDto recipeDto in _recipeDtos)
        {
            char letter = char.ToLowerInvariant(recipeDto.Name[0]);

            if (_recipeCategories.ContainsKey(letter) == false)
            {
                _recipeCategories[letter] = new();
            }

            _recipeCategories[letter].Add(recipeDto);
        }

        foreach (char letter in _recipeCategories.Keys)
        {
            _recipeCategories[letter].Sort((first, second) => string.Compare(first.Name.ToLower(), second.Name.ToLower()));
        }
    }

    private void HandleRecipeNavigation(RecipeDto recipeDto)
    {
        if (NavigationManager == null)
        {
            string errorMessage = "Cannot navigate to recipe page because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo($"recipe/{recipeDto.Id}");
    }
}
