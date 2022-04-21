using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.RecipePage;

public partial class IngredientMeasurementTable : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "IngredientMeasurementTable";

    private List<CheckableIngredientMeasurement> _checkableIngredientMeasurements = new();

    [CascadingParameter]
    public Recipe? RecipePage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        if (RecipePage == null)
        {
            string errorMessage = "Cannot ingredient measurement table rows because recipe page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipePage.RecipeDto == null)
        {
            string errorMessage = "Cannot ingredient measurement table rows because recipe has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

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

    private string GetMeasurement(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (ingredientMeasurementDto == null)
        {
            string errorMessage = "Cannot access ingredient measurement within recipe because passed ingredient measurement has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return ingredientMeasurementDto.Measurement.ToString() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }

    private string GetIngredientMeasurementRowClass(CheckableIngredientMeasurement checkableIngredientMeasurement)
    {
        if (checkableIngredientMeasurement == null)
        {
            string errorMessage = "Cannot set class on ingredient measurement table row because checkable ingredient measurement has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return checkableIngredientMeasurement.IsChecked ? "table-primary" : "";
    }

    private class CheckableIngredientMeasurement
    {
        public bool IsChecked { get; set; } = false;
        public IngredientMeasurementDto IngredientMeasurementDto { get; set; } = new();
    }
}
