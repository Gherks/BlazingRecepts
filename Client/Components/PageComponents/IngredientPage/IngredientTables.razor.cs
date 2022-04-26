using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
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
    protected internal IIngredientService? IngredientService { get; private set; }

    [Inject]
    protected internal IHxMessengerService? MessengerService { get; private set; }

    private void HandleIngredientEditClicked(Guid editingIngredientGuid)
    {
        _editingIngredientGuid = editingIngredientGuid;
        StateHasChanged();
    }

    private void HandleIngredientRemovalModalOpen(IngredientDto? ingredientDto)
    {
        if (_removalConfirmationModal == null)
        {
            const string errorMessage = "Confirmation modal cannot be opened because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ingredientDto == null)
        {
            const string errorMessage = "Cannot start ingredient removal process because ingredient has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(ingredientDto), errorMessage);
        }

        _removalConfirmationModal.Open(ingredientDto, "Ta bort recept", ingredientDto.Name);
    }

    private async Task HandleIngredientEditConfirmed(IngredientDto ingredientDto)
    {
        if (IngredientService == null)
        {
            const string errorMessage = "Cannot save edited ingredient because the ingredient service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        IngredientDto? savedIngredientDto = await IngredientService.SaveAsync(ingredientDto);

        if (savedIngredientDto == null)
        {
            const string errorMessage = "Something went wrong when saving an edited ingredient.";
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
            const string errorMessage = "Ingredient service is not available during ingredient removal.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientsPage == null)
        {
            const string errorMessage = "Cannot remove ingredient from ingredient page collection because ingredient page reference is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientsPage.IngredientCollectionTypes == null)
        {
            const string errorMessage = "Cannot remove ingredient from ingredient page collection because collection is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        bool removalFromDatabaseSuccessful = await IngredientService.DeleteAsync(ingredientDto.Id);

        int categoryIndex = ingredientDto.CategoryDto.SortOrder;
        bool removalFromCollectionSuccessful = IngredientsPage.IngredientCollectionTypes[categoryIndex].Ingredients.Remove(ingredientDto);

        if (removalFromDatabaseSuccessful && removalFromCollectionSuccessful)
        {
            MessengerService.AddInformation("Ingredienser", "Ingrediens borttagen.");
            StateHasChanged();
        }
    }
}
