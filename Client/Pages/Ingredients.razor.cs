using BlazingRecept.Client.Pages.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class Ingredients : PageBase
{
    public IReadOnlyList<IngredientCollectionTypeDto>? IngredientCollectionTypes { get; private set; } = new List<IngredientCollectionTypeDto>();

    [Inject]
    protected internal IIngredientService? IngredientService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        if (IngredientService != null)
        {
            IngredientCollectionTypes = await IngredientService.GetAllSortedAsync();
        }

        IsLoading = false;
    }

    public void AddNewIngredientToCollection(IngredientDto ingredientDto)
    {
        Contracts.LogAndThrowWhenNull(IngredientCollectionTypes, "Cannot add new ingredient to collection because ingredient collection is null.");

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;

        IngredientCollectionTypes[categoryIndex].Ingredients.Add(ingredientDto);
        IngredientCollectionTypes[categoryIndex].Ingredients = IngredientCollectionTypes[categoryIndex].Ingredients
            .OrderBy(ingredientDto => ingredientDto.Name)
            .ToList();

        StateHasChanged();
    }
}
