using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using BlazingRecept.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;

public partial class IngredientMeasurementWorkbenchTable : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "IngredientMeasurementWorkbenchTable";

    [CascadingParameter]
    protected internal RecipeWorkbench? RecipeWorkbench { get; private set; }

    private void HandleIngredientMoveUpInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null)
        {
            const string errorMessage = "RecipeWorkbench page reference has not been set before moving ingredient measurement up in order.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        int movedIndex = RecipeWorkbench.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIndex != 0)
        {
            RecipeWorkbench.ContainedIngredientMeasurements.Swap(movedIndex, movedIndex - 1);
        }

        StateHasChanged();
    }

    private void HandleIngredientMoveDownInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null)
        {
            const string errorMessage = "RecipeWorkbench page reference has not been set before moving ingredient measurement down in order.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        int movedIndex = RecipeWorkbench.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIndex < RecipeWorkbench.ContainedIngredientMeasurements.Count - 1)
        {
            RecipeWorkbench.ContainedIngredientMeasurements.Swap(movedIndex, movedIndex + 1);
        }

        StateHasChanged();
    }

    private async Task HandleIngredientEdit(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null)
        {
            const string errorMessage = "RecipeWorkbench page reference has not been set before trying to edit ingredient measurement.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await RecipeWorkbench.HandleUpdateIngredientModalOpen(ingredientMeasurementDto);
    }

    private async Task HandleIngredientRemoval(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null)
        {
            const string errorMessage = "RecipeWorkbench page reference has not been set before trying to delete ingredient measurement from recipe.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await RecipeWorkbench.OpenIngredientRemovalModalOpen(ingredientMeasurementDto);
    }

    private IReadOnlyList<IngredientMeasurementDto> GetIngredientMeasurementContainedInRecipe()
    {
        if (RecipeWorkbench == null)
        {
            const string errorMessage = "RecipeWorkbench page reference has not been set before trying to access ingredient measurements within recipe.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return RecipeWorkbench.ContainedIngredientMeasurements;
    }

    private string GetMeasurementText(IngredientMeasurementDto ingredientMeasurementDto)
    {
        return ingredientMeasurementDto.Measurement.ToString() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }
}
