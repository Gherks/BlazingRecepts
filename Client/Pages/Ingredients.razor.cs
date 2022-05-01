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
        if (IngredientCollectionTypes == null)
        {
            const string errorMessage = "Cannot add new ingredient to collection because ingredient collection is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;

        IngredientCollectionTypes[categoryIndex].Ingredients.Add(ingredientDto);
        IngredientCollectionTypes[categoryIndex].Ingredients = IngredientCollectionTypes[categoryIndex].Ingredients
            .OrderBy(ingredientDto => ingredientDto.Name)
            .ToList();

        StateHasChanged();
    }
}
