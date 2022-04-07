using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class IngredientTables : ComponentBase
{
    private Guid _editingIngredientGuid = Guid.Empty;

    private RemovalConfirmationModal<IngredientDto>? _removalConfirmationModal;

    [CascadingParameter]
    protected internal Ingredients? IngredientsPage { get; private set; }

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    public void Refresh()
    {
        StateHasChanged();
    }

    private void HandleIngredientEditClicked(Guid editingIngredientGuid)
    {
        _editingIngredientGuid = editingIngredientGuid;
        StateHasChanged();
    }

    private void HandleIngredientRemovalModalOpen(IngredientDto? ingredientDto)
    {
        if (_removalConfirmationModal == null) throw new InvalidOperationException("ConfirmationModal cannot be opened because it has not been set.");
        if (ingredientDto == null) throw new InvalidOperationException();

        _removalConfirmationModal.Open(ingredientDto, "Remove ingredient", ingredientDto.Name);
    }

    private async Task HandleIngredientEditConfirmed(IngredientDto ingredientDto)
    {
        if (IngredientService == null) throw new InvalidOperationException("Cannot save edited ingredient because the ingredient service has not been set.");

        IngredientDto? savedIngredientDto = await IngredientService.SaveAsync(ingredientDto);

        if (savedIngredientDto == null)
        {
            // Log
        }

        _editingIngredientGuid = Guid.Empty;
        StateHasChanged();
    }

    private void HandleIngredientEditCancel()
    {
        _editingIngredientGuid = Guid.Empty;
        StateHasChanged();
    }

    private async Task HandleIngredientRemovalConfirmed(IngredientDto ingredientDto)
    {
        if (IngredientService == null) throw new InvalidOperationException();
        if (IngredientsPage == null) throw new InvalidOperationException("Cannot remove ingredient from ingredient page collection because ingredient page reference is null.");
        if (IngredientsPage.IngredientCollectionTypes == null) throw new InvalidOperationException("Cannot remove ingredient from ingredient page collection because ingredient page collection is null.");

        bool removalFromDatabaseSuccessful = await IngredientService.DeleteAsync(ingredientDto.Id);

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;
        bool removalFromCollectionSuccessful = IngredientsPage.IngredientCollectionTypes[categoryIndex].Ingredients.Remove(ingredientDto);

        if (removalFromDatabaseSuccessful && removalFromCollectionSuccessful)
        {
            if (ToastService == null) throw new InvalidOperationException();

            ToastService.ShowInfo("Successfully removed ingredient.");
            StateHasChanged();
        }
    }
}
