using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.RecipePage;

public partial class IngredientMeasurementTable : PageComponentBase
{
    private List<CheckableIngredientMeasurement> _checkableIngredientMeasurements = new();

    [CascadingParameter]
    protected internal Recipe? RecipePage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot ingredient measurement table rows because recipe page reference has not been set.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot ingredient measurement table rows because recipe has not been set.");

        foreach (IngredientMeasurementDto ingredientMeasurementDto in RecipePage.RecipeDto.IngredientMeasurementDtos)
        {
            _checkableIngredientMeasurements.Add(new()
            {
                IsChecked = false,
                IngredientMeasurementDto = ingredientMeasurementDto
            });
        }

        IsLoading = false;
    }

    private void HandleRowClick(CheckableIngredientMeasurement checkableIngredientMeasurement)
    {
        checkableIngredientMeasurement.IsChecked = !checkableIngredientMeasurement.IsChecked;
        StateHasChanged();
    }

    private string GetMeasurement(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(ingredientMeasurementDto, "Cannot access ingredient measurement within recipe because passed ingredient measurement has not been set.");

        return ingredientMeasurementDto.Measurement.ToString() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }

    private string GetIngredientMeasurementRowClass(CheckableIngredientMeasurement checkableIngredientMeasurement)
    {
        Contracts.LogAndThrowWhenNull(checkableIngredientMeasurement, "Cannot set class on ingredient measurement table row because checkable ingredient measurement has not been set.");

        return checkableIngredientMeasurement.IsChecked ? "table-primary" : "";
    }

    private class CheckableIngredientMeasurement
    {
        public bool IsChecked { get; set; } = false;
        public IngredientMeasurementDto IngredientMeasurementDto { get; set; } = new();
    }
}
