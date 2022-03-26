using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class IngredientTables : ComponentBase
{
    private Guid _editingIngredientGuid = Guid.Empty;

    private RemovalConfirmationModal? _removalConfirmationModal;

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

    private void OnIngredientEditClick(Guid editingIngredientGuid)
    {
        _editingIngredientGuid = editingIngredientGuid;
        StateHasChanged();
    }

    private void OnIngredientRemovalModalOpen(IngredientDto ingredientDto)
    {
        if (_removalConfirmationModal == null)
        {
            throw new InvalidOperationException("ConfirmationModal can not be opened because it has not been set.");
        }

        _removalConfirmationModal.Open(ingredientDto);
    }

    private async Task OnIngredientEditConfirm(IngredientDto ingredientDto)
    {
        if (IngredientService == null)
        {
            throw new InvalidOperationException("Can not save edited ingredient because the ingredient service has not been set.");
        }

        IngredientDto? savedIngredientDto = await IngredientService.SaveAsync(ingredientDto);

        if (savedIngredientDto == null)
        {
            // Log
        }

        _editingIngredientGuid = Guid.Empty;
        StateHasChanged();
    }

    private void OnIngredientEditCancel()
    {
        _editingIngredientGuid = Guid.Empty;
        StateHasChanged();
    }

    private Task OnIngredientRemoved(IngredientDto ingredientDto)
    {
        if (IngredientsPage == null) throw new InvalidOperationException("Can not remove ingredient from ingredient page collection because ingredient page reference is null.");
        if (IngredientsPage.IngredientCollectionTypes == null) throw new InvalidOperationException("Can not remove ingredient from ingredient page collection because ingredient page collection is null.");

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;
        bool removalSuccessful = IngredientsPage.IngredientCollectionTypes[categoryIndex].Ingredients.Remove(ingredientDto);

        if (removalSuccessful)
        {
            if (ToastService == null) throw new InvalidOperationException();

            ToastService.ShowInfo("Successfully removed ingredient.");
            StateHasChanged();
        }

        return Task.CompletedTask;
    }
}
