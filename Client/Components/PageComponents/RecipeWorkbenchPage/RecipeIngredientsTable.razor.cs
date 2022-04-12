using BlazingRecept.Client.Pages;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;

public partial class RecipeIngredientsTable : ComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipeIngredientsTable";

    [CascadingParameter]
    public RecipeWorkbench? RecipeWorkbench { get; set; }

    private void HandleIngredientFormMoveUpInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null)
        {
            string errorMessage = "RecipeWorkbench page reference has not been set before moving ingredient measurement up in order.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        int movedIngredientFormIndex = RecipeWorkbench.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIngredientFormIndex != 0)
        {
            SwapIngredientMeasurementsPositionInList(movedIngredientFormIndex, movedIngredientFormIndex - 1);
        }

        StateHasChanged();
    }

    private void HandleIngredientFormMoveDownInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null)
        {
            string errorMessage = "RecipeWorkbench page reference has not been set before moving ingredient measurement down in order.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        int movedIngredientFormIndex = RecipeWorkbench.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIngredientFormIndex < RecipeWorkbench.ContainedIngredientMeasurements.Count - 1)
        {
            SwapIngredientMeasurementsPositionInList(movedIngredientFormIndex, movedIngredientFormIndex + 1);
        }

        StateHasChanged();
    }

    private void HandleIngredientEdit(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null)
        {
            string errorMessage = "RecipeWorkbench page reference has not been set before trying to edit ingredient measurement.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        RecipeWorkbench.HandleEditIngredientModalOpen(ingredientMeasurementDto);
    }

    private void HandleIngredientRemoval(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null)
        {
            string errorMessage = "RecipeWorkbench page reference has not been set before trying to delete ingredient measurement from recipe.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        RecipeWorkbench.OpenIngredientRemovalModalOpen(ingredientMeasurementDto);
    }

    private IReadOnlyList<IngredientMeasurementDto> GetIngredientMeasurementContainedInRecipe()
    {
        if (RecipeWorkbench == null)
        {
            string errorMessage = "RecipeWorkbench page reference has not been set before trying to access ingredient measurements within recipe.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return RecipeWorkbench.ContainedIngredientMeasurements;
    }

    private string GetMeasurementText(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (ingredientMeasurementDto == null)
        {
            string errorMessage = "Cannot access passed ingredient measurement data because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(ingredientMeasurementDto), errorMessage);
        }

        return ingredientMeasurementDto.Measurement.ToString() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }

    private void SwapIngredientMeasurementsPositionInList(int first, int second)
    {
        if (RecipeWorkbench == null)
        {
            string errorMessage = "RecipeWorkbench page reference has not been set before trying to switch list location of two ingredient measurements in recipe.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        IngredientMeasurementDto temporary = RecipeWorkbench.ContainedIngredientMeasurements[first];
        RecipeWorkbench.ContainedIngredientMeasurements[first] = RecipeWorkbench.ContainedIngredientMeasurements[second];
        RecipeWorkbench.ContainedIngredientMeasurements[second] = temporary;
    }
}
