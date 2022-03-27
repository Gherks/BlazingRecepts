using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class Index : ComponentBase
{
    private IReadOnlyList<RecipeDto>? _recipeDtos = new List<RecipeDto>();

    [Inject]
    public IRecipeService? RecipeService { get; set; }

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (RecipeService == null) throw new InvalidOperationException();

        _recipeDtos = await RecipeService.GetAllAsync();
    }

    private void HandleRecipeNavigation(RecipeDto recipeDto)
    {
        if (NavigationManager == null) throw new InvalidOperationException();

        NavigationManager.NavigateTo($"recipes/{recipeDto.Id}");
    }
}
