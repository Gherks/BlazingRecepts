using BlazingRecept.Client.Pages;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;

public partial class RecipeIngredientsTable : ComponentBase
{
    [CascadingParameter]
    public RecipeWorkbench? RecipeWorkbench { get; set; }

    private void HandleIngredientFormMoveUpInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null) throw new InvalidOperationException();

        int movedIngredientFormIndex = RecipeWorkbench.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIngredientFormIndex != 0)
        {
            SwapIngredientMeasurementsPositionInList(movedIngredientFormIndex, movedIngredientFormIndex - 1);
        }

        StateHasChanged();
    }

    private void HandleIngredientFormMoveDownInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null) throw new InvalidOperationException();

        int movedIngredientFormIndex = RecipeWorkbench.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIngredientFormIndex < RecipeWorkbench.ContainedIngredientMeasurements.Count - 1)
        {
            SwapIngredientMeasurementsPositionInList(movedIngredientFormIndex, movedIngredientFormIndex + 1);
        }

        StateHasChanged();
    }

    private void HandleIngredientEdit(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null) throw new InvalidOperationException();

        RecipeWorkbench.HandleEditIngredientModalOpen(ingredientMeasurementDto);
    }

    private void HandleIngredientRemoval(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (RecipeWorkbench == null) throw new InvalidOperationException();

        RecipeWorkbench.OpenIngredientRemovalModalOpen(ingredientMeasurementDto);
    }

    private IReadOnlyList<IngredientMeasurementDto> GetIngredientForms()
    {
        if (RecipeWorkbench == null) throw new InvalidOperationException();

        return RecipeWorkbench.ContainedIngredientMeasurements;
    }

    private string GetMeasurement(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (ingredientMeasurementDto == null) throw new InvalidOperationException();

        return ingredientMeasurementDto.Measurement.Trim() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }

    private void SwapIngredientMeasurementsPositionInList(int first, int second)
    {
        if (RecipeWorkbench == null) throw new InvalidOperationException();

        IngredientMeasurementDto temporary = RecipeWorkbench.ContainedIngredientMeasurements[first];
        RecipeWorkbench.ContainedIngredientMeasurements[first] = RecipeWorkbench.ContainedIngredientMeasurements[second];
        RecipeWorkbench.ContainedIngredientMeasurements[second] = temporary;
    }
}
