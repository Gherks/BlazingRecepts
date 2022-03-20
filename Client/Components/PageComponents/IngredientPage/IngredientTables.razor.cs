using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using BlazingRecept.Client.Pages;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class IngredientTables : ComponentBase
{
    private RemovalConfirmationModal? _removalConfirmationModal;

    [CascadingParameter]
    protected internal Ingredients? IngredientsPage { get; private set; }

    public void Refresh()
    {
        StateHasChanged();
    }

    private void OnIngredientEditClick()
    {

    }

    private void OnIngredientRemovalModalOpen(IngredientDto ingredientDto)
    {
        if (_removalConfirmationModal == null)
        {
            throw new InvalidOperationException("ConfirmationModal can not be opened because it has not been set.");
        }

        _removalConfirmationModal.Open(ingredientDto);
    }

    private Task OnIngredientRemoved(IngredientDto ingredientDto)
    {
        if (IngredientsPage == null) throw new InvalidOperationException("Can not remove ingredient from ingredient page collection because ingredient page reference is null.");
        if (IngredientsPage.IngredientCollectionTypes == null) throw new InvalidOperationException("Can not remove ingredient from ingredient page collection because ingredient page collection is null.");

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;
        bool removalSuccessful = IngredientsPage.IngredientCollectionTypes[categoryIndex].Ingredients.Remove(ingredientDto);

        if (removalSuccessful)
        {
            StateHasChanged();
        }

        return Task.CompletedTask;
    }
}
