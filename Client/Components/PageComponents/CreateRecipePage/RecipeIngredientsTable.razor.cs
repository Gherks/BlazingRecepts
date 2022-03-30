using BlazingRecept.Client.Pages;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.CreateRecipePage;

public partial class RecipeIngredientsTable : ComponentBase
{
    [CascadingParameter]
    public CreateRecipe? CreateRecipe { get; set; }

    private void HandleIngredientFormMoveUpInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        int movedIngredientFormIndex = CreateRecipe.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIngredientFormIndex != 0)
        {
            SwapIngredientMeasurementsPositionInList(movedIngredientFormIndex, movedIngredientFormIndex - 1);
        }

        StateHasChanged();
    }

    private void HandleIngredientFormMoveDownInOrder(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        int movedIngredientFormIndex = CreateRecipe.ContainedIngredientMeasurements.IndexOf(ingredientMeasurementDto);

        if (movedIngredientFormIndex < CreateRecipe.ContainedIngredientMeasurements.Count - 1)
        {
            SwapIngredientMeasurementsPositionInList(movedIngredientFormIndex, movedIngredientFormIndex + 1);
        }

        StateHasChanged();
    }

    private void HandleIngredientEdit(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        CreateRecipe.HandleEditIngredientModalOpen(ingredientMeasurementDto);
    }

    private void HandleIngredientRemoval(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        CreateRecipe.OpenIngredientRemovalModalOpen(ingredientMeasurementDto);
    }

    IReadOnlyList<IngredientMeasurementDto> GetIngredientForms()
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        return CreateRecipe.ContainedIngredientMeasurements;
    }

    private void SwapIngredientMeasurementsPositionInList(int first, int second)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        IngredientMeasurementDto temporary = CreateRecipe.ContainedIngredientMeasurements[first];
        CreateRecipe.ContainedIngredientMeasurements[first] = CreateRecipe.ContainedIngredientMeasurements[second];
        CreateRecipe.ContainedIngredientMeasurements[second] = temporary;
    }
}
