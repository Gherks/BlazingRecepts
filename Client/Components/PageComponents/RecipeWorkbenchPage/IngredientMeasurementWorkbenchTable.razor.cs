using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Contract;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using BlazingRecept.Shared.Extensions;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;

public partial class IngredientMeasurementWorkbenchTable : PageComponentBase
{
    [CascadingParameter]
    protected internal RecipeWorkbench? RecipeWorkbench { get; private set; }

    private void HandleIngredientMoveUpInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipeWorkbench, "RecipeWorkbench page reference has not been set before moving ingredient measurement up in order.");

        int movedIndex = RecipeWorkbench.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIndex != 0)
        {
            RecipeWorkbench.ContainedIngredientMeasurements.Swap(movedIndex, movedIndex - 1);
        }

        StateHasChanged();
    }

    private void HandleIngredientMoveDownInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipeWorkbench, "RecipeWorkbench page reference has not been set before moving ingredient measurement down in order.");

        int movedIndex = RecipeWorkbench.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIndex < RecipeWorkbench.ContainedIngredientMeasurements.Count - 1)
        {
            RecipeWorkbench.ContainedIngredientMeasurements.Swap(movedIndex, movedIndex + 1);
        }

        StateHasChanged();
    }

    private async Task HandleIngredientEdit(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipeWorkbench, "RecipeWorkbench page reference has not been set before trying to edit ingredient measurement.");

        await RecipeWorkbench.HandleUpdateIngredientModalOpen(ingredientMeasurementDto);
    }

    private async Task HandleIngredientRemoval(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipeWorkbench, "RecipeWorkbench page reference has not been set before trying to delete ingredient measurement from recipe.");

        await RecipeWorkbench.OpenIngredientRemovalModalOpen(ingredientMeasurementDto);
    }

    private IReadOnlyList<IngredientMeasurementDto> GetIngredientMeasurementContainedInRecipe()
    {
        Contracts.LogAndThrowWhenNull(RecipeWorkbench, "RecipeWorkbench page reference has not been set before trying to access ingredient measurements within recipe.");

        return RecipeWorkbench.ContainedIngredientMeasurements;
    }

    private string GetMeasurementText(IngredientMeasurementDto ingredientMeasurementDto)
    {
        return ingredientMeasurementDto.Measurement.ToString() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }
}
