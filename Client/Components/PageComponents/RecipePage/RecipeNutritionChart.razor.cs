using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Extensions;
using BlazingRecept.Client.Pages;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.RecipePage;

public partial class RecipeNutritionChart : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipeNutritionChart";

    private NutritionalChartItem[]? _nutritionalChartItems = null;

    [CascadingParameter]
    protected internal Recipe? RecipePage { get; private set; }

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

        LoadChartItems();

        IsLoading = false;
    }

    private void LoadChartItems()
    {
        if (RecipePage == null)
        {
            string errorMessage = "Cannot construct nutritional chart items because recipe page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipePage.RecipeDto == null)
        {
            string errorMessage = "Cannot construct nutritional chart items because recipe has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _nutritionalChartItems = new NutritionalChartItem[] {
            new()
            {
                Label = "Fett",
                Value = RecipePage.RecipeDto.GetTotalFat()
            },
            new()
            {
                Label = "Kolhydrater",
                Value = RecipePage.RecipeDto.GetTotalCarbohydrates()
            },
            new()
            {
                Label = "Protein",
                Value = RecipePage.RecipeDto.GetTotalProtein()
            }
        };
    }

    private class NutritionalChartItem
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; } = 0.0;
    }
}
