using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.Ingredients;

public partial class IngredientTables : ComponentBase
{
    IReadOnlyList<IngredientCollectionTypeDto>? _ingredientCollectionTypes = new List<IngredientCollectionTypeDto>();

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (IngredientService != null)
        {
            _ingredientCollectionTypes = await IngredientService.GetAllSortedAsync();
        }
    }

    private void OnIngredientEditClick()
    {

    }

    private void OnIngredientRemovalModalOpen()
    {

    }
}
