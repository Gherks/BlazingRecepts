using BlazingRecept.Client.Components.PageComponents.IngredientPage;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class Ingredients : ComponentBase
{
    private IngredientTables? _ingredientTables;

    public IReadOnlyList<IngredientCollectionTypeDto>? IngredientCollectionTypes { get; private set; } = new List<IngredientCollectionTypeDto>();

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (IngredientService != null)
        {
            IngredientCollectionTypes = await IngredientService.GetAllSortedAsync();
        }
    }

    public void AddNewIngredientToCollection(IngredientDto ingredientDto)
    {
        if (IngredientCollectionTypes == null) throw new InvalidOperationException("Cannot add new ingredient to collection because ingredient collection is null.");
        if (_ingredientTables == null) throw new InvalidOperationException("Ingredient table reference is null and can therefore not be refreshed.");

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;

        IngredientCollectionTypes[categoryIndex].Ingredients.Add(ingredientDto);
        IngredientCollectionTypes[categoryIndex].Ingredients = IngredientCollectionTypes[categoryIndex].Ingredients
            .OrderBy(ingredientDto => ingredientDto.Name)
            .ToList();

        _ingredientTables.Refresh();
    }
}
