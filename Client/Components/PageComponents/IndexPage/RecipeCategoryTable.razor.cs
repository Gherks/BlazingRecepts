using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.IndexPage;

public partial class RecipeCategoryTable : ComponentBase
{
    private IReadOnlyList<RecipeDto>? _recipeDtos = new List<RecipeDto>();

    private Dictionary<char, List<RecipeDto>> _recipeCategories = new();

    [Inject]
    public IRecipeService? RecipeService { get; set; }

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (RecipeService == null) throw new InvalidOperationException();

        _recipeDtos = await RecipeService.GetAllAsync();

        CategorizeByName();
    }

    private void CategorizeByName()
    {
        if (_recipeDtos == null) throw new InvalidOperationException();

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
        if (NavigationManager == null) throw new InvalidOperationException();

        NavigationManager.NavigateTo($"recipe/{recipeDto.Id}");
    }
}
