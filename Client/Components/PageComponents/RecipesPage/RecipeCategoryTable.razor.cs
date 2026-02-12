using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Contract;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.RecipesPage;

public partial class RecipeCategoryTable : PageComponentBase
{
    private IReadOnlyList<RecipeDto>? _recipeDtos = new List<RecipeDto>();

    private Dictionary<char, List<RecipeDto>> _recipeCategories = new();

    [Inject]
    protected internal IRecipeService? RecipeService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Contracts.LogAndThrowWhenNull(RecipeService, "Cannot initialize RecipeCategoryTable because recipe service has not been set.");

        IsLoading = true;

        await base.OnInitializedAsync();

        _recipeDtos = await RecipeService.GetAllAsync();

        CategorizeByName();

        IsLoading = false;
    }

    private void CategorizeByName()
    {
        Contracts.LogAndThrowWhenNull(_recipeDtos, "Cannot categorize recipes because there are no recipes to categorize.");

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
}
