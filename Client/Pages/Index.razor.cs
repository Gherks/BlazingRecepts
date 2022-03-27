using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class Index : ComponentBase
{
    public IReadOnlyList<RecipeDto>? _recipes = new List<RecipeDto>();

    [Inject]
    public IRecipeService? RecipeService { get; set; }

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (RecipeService == null) throw new InvalidOperationException();

        _recipes = await RecipeService.GetAllAsync();
    }

    private void HandleRecipeNavigation(RecipeDto recipeDto)
    {
        if (NavigationManager == null) throw new InvalidOperationException();

        NavigationManager.NavigateTo($"recipes/{recipeDto.Id}");
    }
}
