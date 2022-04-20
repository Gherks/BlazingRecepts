using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class IngredientTables : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "IngredientTables";

    private Guid _editingIngredientGuid = Guid.Empty;

    private RemovalConfirmationModal<IngredientDto>? _removalConfirmationModal;

    [CascadingParameter]
    protected internal Ingredients? IngredientsPage { get; private set; }

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    private void HandleIngredientEditClicked(Guid editingIngredientGuid)
    {
        _editingIngredientGuid = editingIngredientGuid;
        StateHasChanged();
    }

    private void HandleIngredientRemovalModalOpen(IngredientDto? ingredientDto)
    {
        if (_removalConfirmationModal == null)
        {
            string errorMessage = "Confirmation modal cannot be opened because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ingredientDto == null)
        {
            string errorMessage = "Cannot start ingredient removal process because ingredient has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(ingredientDto), errorMessage);
        }

        _removalConfirmationModal.Open(ingredientDto, "Ta bort recept", ingredientDto.Name);
    }

    private async Task HandleIngredientEditConfirmed(IngredientDto ingredientDto)
    {
        if (IngredientService == null)
        {
            string errorMessage = "Cannot save edited ingredient because the ingredient service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        IngredientDto? savedIngredientDto = await IngredientService.SaveAsync(ingredientDto);

        if (savedIngredientDto == null)
        {
            string errorMessage = "Something went wrong when saving an edited ingredient.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
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
        if (IngredientService == null)
        {
            string errorMessage = "Ingredient service is not available during ingredient removal.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientsPage == null)
        {
            string errorMessage = "Cannot remove ingredient from ingredient page collection because ingredient page reference is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientsPage.IngredientCollectionTypes == null)
        {
            string errorMessage = "Cannot remove ingredient from ingredient page collection because collection is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ToastService == null)
        {
            string errorMessage = "Toast service is not available during ingredient removal.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        bool removalFromDatabaseSuccessful = await IngredientService.DeleteAsync(ingredientDto.Id);

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;
        bool removalFromCollectionSuccessful = IngredientsPage.IngredientCollectionTypes[categoryIndex].Ingredients.Remove(ingredientDto);

        if (removalFromDatabaseSuccessful && removalFromCollectionSuccessful)
        {
            ToastService.ShowInfo("Ingrediens borttagen.");
            StateHasChanged();
        }
    }
}
