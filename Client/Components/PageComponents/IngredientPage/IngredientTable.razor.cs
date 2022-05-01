using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Extensions;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class IngredientTable : PageComponentBase
{
    private const string _logProperty = "Domain";
    private const string _logDomainName = "IngredientTables";
    private const string _collapseButtonTextExpanded = "Minimera";
    private const string _collapseButtonTextCollapsed = "Expandera";
    private const string _collapseIdPrefix = "Collapsable_";
    private const string _collapseButtonStyle = "width: 128px;";

    private string _collapseButtonText = _collapseButtonTextExpanded;
    private IngredientDto? _uneditedIngredientDto = null;

    private RemovalConfirmationModal<IngredientDto>? _removalConfirmationModal;

    [Parameter]
    public IngredientCollectionTypeDto? IngredientCollectionTypeDto { get; set; }

    [Inject]
    protected internal IIngredientService? IngredientService { get; private set; }

    [Inject]
    protected internal IHxMessengerService? MessengerService { get; private set; }

    private string GetCollapsableTableTargetId(IngredientCollectionTypeDto ingredientCollectionTypeDto)
    {
        return $"#{_collapseIdPrefix}{ingredientCollectionTypeDto.Name}";
    }

    private string GetCollapsableTableId(IngredientCollectionTypeDto ingredientCollectionTypeDto)
    {
        return $"{_collapseIdPrefix}{ingredientCollectionTypeDto.Name}";
    }

    private async Task HandleCollapseShown(string collapseId)
    {
        _collapseButtonText = _collapseButtonTextExpanded;

        await Task.CompletedTask;
    }

    private async Task HandleCollapseHidden(string collapseId)
    {
        _collapseButtonText = _collapseButtonTextCollapsed;

        await Task.CompletedTask;
    }

    private void HandleIngredientEditClicked(IngredientDto editingIngredientDto)
    {
        _uneditedIngredientDto = new()
        {
            Id = editingIngredientDto.Id,
            Name = editingIngredientDto.Name,
            Fat = editingIngredientDto.Fat,
            Carbohydrates = editingIngredientDto.Carbohydrates,
            Protein = editingIngredientDto.Protein,
            Calories = editingIngredientDto.Calories,
            CategoryDto = editingIngredientDto.CategoryDto
        };

        StateHasChanged();
    }

    private void HandleIngredientRemovalModalOpen(IngredientDto? ingredientDto)
    {
        if (ingredientDto == null)
        {
            const string errorMessage = "Cannot start ingredient removal process because ingredient has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(ingredientDto), errorMessage);
        }

        if (_removalConfirmationModal == null)
        {
            const string errorMessage = "Confirmation modal cannot be opened because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
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

        MessengerService.AddSuccess("Ingredienser", "Ingrediens uppdaterad!");
        _uneditedIngredientDto = null;

        StateHasChanged();
    }

    private void HandleIngredientEditCancel(IngredientDto editedIngredientDto)
    {
        if (_uneditedIngredientDto == null)
        {
            const string errorMessage = "Cannot cancel ingredient table inline editing because unedited ingredient dto is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        editedIngredientDto.Name = _uneditedIngredientDto.Name;
        editedIngredientDto.Fat = _uneditedIngredientDto.Fat;
        editedIngredientDto.Carbohydrates = _uneditedIngredientDto.Carbohydrates;
        editedIngredientDto.Protein = _uneditedIngredientDto.Protein;
        editedIngredientDto.Calories = _uneditedIngredientDto.Calories;

        _uneditedIngredientDto = null;
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

        if (IngredientCollectionTypeDto == null)
        {
            const string errorMessage = "Cannot remove ingredient from collection because ingredient collection parameter has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        bool removalFromDatabaseSuccessful = await IngredientService.DeleteAsync(ingredientDto.Id);
        bool removalFromCollectionSuccessful = IngredientCollectionTypeDto.Ingredients.Remove(ingredientDto);

        if (removalFromDatabaseSuccessful && removalFromCollectionSuccessful)
        {
            MessengerService.AddInformation("Ingredienser", "Ingrediens borttagen.");
            StateHasChanged();
        }
    }
}
