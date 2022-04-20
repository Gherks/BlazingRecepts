using BlazingRecept.Client.Components.PageComponents.IngredientPage;
using BlazingRecept.Client.Pages.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Pages;

public partial class Ingredients : PageBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "IngredientsPage";

    private IngredientTables? _ingredientTables;

    public IReadOnlyList<IngredientCollectionTypeDto>? IngredientCollectionTypes { get; private set; } = new List<IngredientCollectionTypeDto>();

    [Inject]
    public IIngredientService? IngredientService { get; set; }

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
        if (IngredientCollectionTypes == null)
        {
            string errorMessage = "Cannot add new ingredient to collection because ingredient collection is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_ingredientTables == null)
        {
            string errorMessage = "Ingredient table reference is null and can therefore not be refreshed.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;

        IngredientCollectionTypes[categoryIndex].Ingredients.Add(ingredientDto);
        IngredientCollectionTypes[categoryIndex].Ingredients = IngredientCollectionTypes[categoryIndex].Ingredients
            .OrderBy(ingredientDto => ingredientDto.Name)
            .ToList();

        _ingredientTables.Refresh();
    }
}
